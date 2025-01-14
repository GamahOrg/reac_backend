using AutoMapper;
using Core.Contracts;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Notification;
using Wafi.Application.Hubs;
using Wafi.Application.Hubs.Contracts;

namespace REAC.Application.AppServices.System
{
    public partial class NotificationPlanService : BasicService<SysNotificationPlan, Guid>, ISysNotificationPlanService
    {
        private readonly IJwtReader _jwtReader;
        private readonly ICitizenService _citizen;
        private readonly IHubContext<NotificationHub, INotificationClient> _notificationHub;
        private readonly ISysFileManagerService _fileService;
        private readonly ISMSService _sMSService;
        private readonly ISysSettingService _settingService;
        private readonly IMailService _mailService;

        public NotificationPlanService(IMailService mailService,
            ISysSettingService settingService,
            ISMSService sMSService,
            ISysFileManagerService fileService,
            IEntityRepository repository,
            IMapper mapper,
            ILoggerManager loggerManager,
            IJwtReader jwtReader,
            ICitizenService citizen,
            IHubContext<NotificationHub, INotificationClient> notificationHub)

            : base(repository, mapper, loggerManager, jwtReader)
        {
            _fileService = fileService;
            _jwtReader = jwtReader;
            _mapper = mapper;
            _citizen = citizen;
            _notificationHub = notificationHub;
            _sMSService = sMSService;
            _mailService = mailService;
            _settingService = settingService;
        }

        public async Task<BaseResponse> ExecuteNotificationPlan(Guid notificationPlanId, Guid requestId)
        {
            var notificationPlan = (await _repository.GetQueryable<SysNotificationPlan, Guid>())
                                    .Include(x => x.SysNotificationPlanItems)
                                    .ThenInclude(x => x.SysNotificationPlanItemDestinations)
                                    .Include(x => x.SysNotificationPlanItems)
                                    .ThenInclude(x => x.SysNotificationPlanItemDestinations)
                                    .ThenInclude(x => x.SysDepartment)
                                    .ThenInclude(x => x.SysEmployees)
                                    .Where(x => x.Id == notificationPlanId).ToList().FirstOrDefault();

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                    .Include(x => x.ApplicantCitizen)
                                    .Include(x => x.Contract)
                                    .ThenInclude(x => x.FirstPartyFacility)
                                    .ThenInclude(x => x.Citizen)
                                    .Include(x => x.Contract)
                                    .ThenInclude(x => x.SecondPartyFacility)
                                    .ThenInclude(x => x.Citizen)
                                    .Include(x => x.Contract)
                                    .ThenInclude(x => x.FirstPartyCitizen)
                                    .Include(x => x.Contract)
                                    .ThenInclude(x => x.SecondPartyCitizen)
                                    .Where(x => x.Id == notificationPlanId).ToList().FirstOrDefault();


            if (notificationPlan?.SysNotificationPlanItems == null)
                return new BaseResponse() { IsDone = true };


            foreach (var item in notificationPlan?.SysNotificationPlanItems)
            {
                if (item?.SysNotificationPlanItemDestinations != null)
                {
                    foreach (var destination in item?.SysNotificationPlanItemDestinations)
                    {
                        string spplicationNotificationTemplate = "";
                        string mailTemplate = "";
                        string smsTemplate = "";

                        if (item.ApplicationNotificationTemplate.IsNullOrEmpty())
                            spplicationNotificationTemplate = SerializeMessage(item.ApplicationNotificationTemplate, litigationRequest, destination);

                        if (item.MailTemplate.IsNullOrEmpty())
                            mailTemplate = SerializeMessage(item.MailTemplate, litigationRequest, destination);

                        if (item.SmsTemplate.IsNullOrEmpty())
                            smsTemplate = SerializeMessage(item.SmsTemplate, litigationRequest, destination);

                        switch (destination.NotificationDestinationType)
                        {
                            case NotificationDestinationType.Arbitrator:
                                switch (destination.NotificationType)
                                {
                                    case NotificationType.Application:
                                        await SendApplicationNotification(destination.NotificationActorTypeId, litigationRequest?.ReformerCitizen?.Id, RequestType.LitigationRequest, requestId, spplicationNotificationTemplate);
                                        break;
                                    case NotificationType.Mail:
                                        if (litigationRequest?.ReformerCitizen?.Email != null & mailTemplate != "")
                                            SendMail(item.EmailSubject, mailTemplate, litigationRequest?.ReformerCitizen?.Email);
                                        break;
                                    case NotificationType.SMS:
                                        if (litigationRequest?.ReformerCitizen?.Mobile != null & smsTemplate != "")
                                            SendSmsAsync(smsTemplate, litigationRequest?.ReformerCitizen?.Mobile);
                                        break;
                                }
                                break;
                            case NotificationDestinationType.Reformer:
                                switch (destination.NotificationType)
                                {
                                    case NotificationType.Application:
                                        await SendApplicationNotification(destination.NotificationActorTypeId, litigationRequest?.ReformerCitizen?.Id, RequestType.LitigationRequest, requestId, spplicationNotificationTemplate);
                                        break;
                                    case NotificationType.Mail:
                                        if (litigationRequest?.ReformerCitizen?.Email != null & mailTemplate != "")
                                            SendMail(item.EmailSubject, mailTemplate, litigationRequest?.ReformerCitizen?.Email);
                                        break;
                                    case NotificationType.SMS:
                                        if (litigationRequest?.ReformerCitizen?.Mobile != null & smsTemplate != "")
                                            SendSmsAsync(smsTemplate, litigationRequest?.ReformerCitizen?.Mobile);
                                        break;
                                }
                                break;
                            case NotificationDestinationType.ApplicantCitizen:
                                switch (destination.NotificationType)
                                {
                                    case NotificationType.Application:
                                        await SendApplicationNotification(destination.NotificationActorTypeId, litigationRequest?.ApplicantCitizen?.Id, RequestType.LitigationRequest, requestId, spplicationNotificationTemplate);
                                        break;
                                    case NotificationType.Mail:
                                        if (litigationRequest?.ApplicantCitizen?.Email != null & mailTemplate != "")
                                            SendMail(item.EmailSubject, mailTemplate, litigationRequest?.ApplicantCitizen?.Email);
                                        break;
                                    case NotificationType.SMS:
                                        if (litigationRequest?.ApplicantCitizen?.Mobile != null & smsTemplate != "")
                                            SendSmsAsync(smsTemplate, litigationRequest?.ApplicantCitizen?.Mobile);
                                        break;
                                }
                                break;
                            case NotificationDestinationType.Defendant:
                                switch (destination.NotificationType)
                                {
                                    case NotificationType.Application:
                                        await SendApplicationNotification(destination.NotificationActorTypeId, litigationRequest?.Contract.SecondPartyCitizen?.Id, RequestType.LitigationRequest, requestId, spplicationNotificationTemplate);
                                        break;
                                    case NotificationType.Mail:
                                        if (litigationRequest?.Contract?.SecondPartyCitizen?.Email != null & mailTemplate != "")
                                            SendMail(item.EmailSubject, smsTemplate, litigationRequest?.Contract.SecondPartyCitizen?.Email);
                                        break;
                                    case NotificationType.SMS:
                                        if (litigationRequest?.Contract?.SecondPartyCitizen?.Mobile != null & smsTemplate != "")
                                            SendSmsAsync(smsTemplate, litigationRequest?.Contract.SecondPartyCitizen?.Mobile);
                                        break;
                                }
                                break;
                            case NotificationDestinationType.Plaintiff:
                                switch (destination.NotificationType)
                                {
                                    case NotificationType.Application:
                                        await SendApplicationNotification(destination.NotificationActorTypeId, litigationRequest?.Contract.FirstPartyCitizen?.Id, RequestType.LitigationRequest, requestId, spplicationNotificationTemplate);
                                        break;
                                    case NotificationType.Mail:
                                        if (litigationRequest?.Contract?.FirstPartyCitizen?.Email != null & mailTemplate != "")
                                            SendMail(item.EmailSubject, smsTemplate, litigationRequest?.Contract.FirstPartyCitizen?.Email);
                                        break;
                                    case NotificationType.SMS:
                                        if (litigationRequest?.Contract?.FirstPartyCitizen?.Mobile != null & smsTemplate != "")
                                            SendSmsAsync(smsTemplate, litigationRequest?.Contract.FirstPartyCitizen?.Mobile);
                                        break;
                                }
                                break;
                            case NotificationDestinationType.DepartmentAdmin:
                                switch (destination.NotificationType)
                                {
                                    case NotificationType.Application:
                                        var admin = destination.SysDepartment?.SysEmployees.Where(t => t.IsAdmin).FirstOrDefault();
                                        await SendApplicationNotification(destination.NotificationActorTypeId, admin?.Id, RequestType.LitigationRequest, requestId, spplicationNotificationTemplate);
                                        break;
                                    case NotificationType.Mail:
                                        if (destination.SysDepartment?.SysEmployees.Where(t => t.IsAdmin).FirstOrDefault()?.Email != null & mailTemplate != "")
                                            SendMail(item.EmailSubject, smsTemplate, destination.SysDepartment?.SysEmployees.Where(t => t.IsAdmin).FirstOrDefault()?.Email);
                                        break;
                                    case NotificationType.SMS:
                                        if (destination.SysDepartment?.SysEmployees.Where(t => t.IsAdmin).FirstOrDefault()?.Mobile != null & smsTemplate != "")
                                            SendSmsAsync(smsTemplate, destination.SysDepartment?.SysEmployees.Where(t => t.IsAdmin).FirstOrDefault()?.Mobile);
                                        break;
                                }
                                break;

                        }

                    }
                }
            }


            return new BaseResponse() { IsDone = true };
        }
        private async Task<BaseResponse> SendApplicationNotification(NotificationActorType notificationActorTypeId, Guid? actorId, RequestType requestTypeId, Guid? requestId, string message)
        {
            if (requestId.HasValue && actorId.HasValue && !string.IsNullOrWhiteSpace(message))
            {
                SysNotification sysNotification = new SysNotification
                {
                    NotificationActorTypeId = notificationActorTypeId,
                    Message = message,
                    ActorId = actorId.Value,
                    RequestId = requestId.Value,
                    RequestTypeId = requestTypeId,
                    CreatedOn = DateTime.Now,
                };

                await _repository.AddAsync<SysNotification, Guid>(sysNotification);
                await _repository.SaveChangesAsync();
                if (notificationActorTypeId == NotificationActorType.Employee)
                {
                    await _notificationHub.Clients.All.NotifyEmployee(_mapper.Map<SysNotificationDto>(sysNotification));
                }
                else
                {
                    await _notificationHub.Clients.All.NotifyCitizen(_mapper.Map<SysNotificationDto>(sysNotification));
                }
            }
            return new BaseResponse() { IsDone = true };
        }

        private async Task<BaseResponse> SendSmsAsync(string message, string? mobile)
        {
            if (message == null)
                return new BaseResponse() { IsDone = true };

            if (mobile == null)
                return new BaseResponse() { IsDone = true };

            await _sMSService.SendSMS(new Contracts.Dtos.Integration.GEE.SMSRequestDto()
            {
                message_text = message,
                recipients = new List<string>() { mobile },
                notification_type = "SMS",
            });

            return new BaseResponse() { IsDone = true };
        }

        private async Task<BaseResponse> SendMail(string? emailSubject, string message, string? mail)
        {
            if (mail == null)
                return new BaseResponse() { IsDone = true };

            await _mailService.SendMail(new List<string>() { mail }, emailSubject, message);
            return new BaseResponse() { IsDone = true };
        }

        private string SerializeMessage(string template, LitigationRequest? litigationRequest, SysNotificationPlanItemDestination notificationPlanItemDestination)
        {
            if (litigationRequest != null)
            {
                template = template.Replace("{Request.RequestCode}", litigationRequest.RequestCode).
                                    Replace("{Request.Contract.ContractKey}", litigationRequest.Contract.ContractKey).
                                    Replace("{Request.AreaName}", litigationRequest.Area.AreaName).
                                    Replace("{Request.CityName}", litigationRequest.City.CityName).
                                    Replace("{Request.LitigationCategoryName}", litigationRequest.LitigationRequestCategory.CategoryName).
                                    Replace("{Request.Title}", litigationRequest.Title).

                                    Replace("{Plaintiff.Citizen.FullName}", litigationRequest.Contract.FirstPartyCitizen?.FullName).
                                    Replace("{Plaintiff.Citizen.ShortName}", litigationRequest.Contract.FirstPartyCitizen?.ShortName).
                                    Replace("{Plaintiff.Citizen.Mobile}", litigationRequest.Contract.FirstPartyCitizen?.Mobile).
                                    Replace("{Plaintiff.Citizen.Email}", litigationRequest.Contract.FirstPartyCitizen?.Email).
                                    Replace("{Plaintiff.Citizen.AreaName}", litigationRequest.Contract.FirstPartyCitizen?.Area?.AreaName).
                                    Replace("{Plaintiff.Citizen.CityName}", litigationRequest.Contract.FirstPartyCitizen?.City?.CityName).

                                    Replace("{Plaintiff.Facility.ArabicFacilityName}", litigationRequest.Contract.FirstPartyFacility?.ArabicFacilityName).
                                    Replace("{Plaintiff.Facility.Facility700Number}", litigationRequest.Contract.FirstPartyFacility?.Facility700Number).
                                    Replace("{Plaintiff.Facility.CommercialRegisterNumber}", litigationRequest.Contract.FirstPartyFacility?.CommercialRegisterNumber).

                                    Replace("{Defendant.Citizen.FullName}", litigationRequest.Contract.SecondPartyCitizen?.FullName).
                                    Replace("{Defendant.Citizen.ShortName}", litigationRequest.Contract.SecondPartyCitizen?.ShortName).
                                    Replace("{Defendant.Citizen.Mobile}", litigationRequest.Contract.SecondPartyCitizen?.Mobile).
                                    Replace("{Defendant.Citizen.Email}", litigationRequest.Contract.SecondPartyCitizen?.Email).
                                    Replace("{Defendant.Citizen.AreaName}", litigationRequest.Contract.SecondPartyCitizen?.Area?.AreaName).
                                    Replace("{Defendant.Citizen.CityName}", litigationRequest.Contract.SecondPartyCitizen?.City.CityName).

                                    Replace("{Defendant.Facility.ArabicFacilityName}", litigationRequest.Contract.SecondPartyFacility?.ArabicFacilityName).
                                    Replace("{Defendant.Facility.Facility700Number}", litigationRequest.Contract.SecondPartyFacility?.Facility700Number).
                                    Replace("{Defendant.Facility.CommercialRegisterNumber}", litigationRequest.Contract.SecondPartyFacility?.CommercialRegisterNumber).

                                    Replace("{DepartmentAdmin.FullName}", notificationPlanItemDestination.SysDepartment?.SysEmployees.Where(t => t.IsAdmin).FirstOrDefault()?.EmployeeName).
                                    Replace("{DepartmentAdmin.Mobile}", notificationPlanItemDestination.SysDepartment?.SysEmployees.Where(t => t.IsAdmin).FirstOrDefault()?.Mobile).
                                    Replace("{DepartmentAdmin.Email}", notificationPlanItemDestination.SysDepartment?.SysEmployees.Where(t => t.IsAdmin).FirstOrDefault()?.Email).

                                    Replace("{Reformer.FullName}", litigationRequest.ReformerCitizen?.FullName).
                                    Replace("{Reformer.ShortName}", litigationRequest.ReformerCitizen?.ShortName).
                                    Replace("{Reformer.Mobile}", litigationRequest.ReformerCitizen?.Mobile).
                                    Replace("{Reformer.Email}", litigationRequest.ReformerCitizen?.Email).
                                    Replace("{Reformer.AreaName}", litigationRequest.ReformerCitizen?.Area?.AreaName).
                                    Replace("{Reformer.CityName}", litigationRequest.ReformerCitizen?.City?.CityName).

                                    //Replace("{Arbitrator.FullName}", "").
                                    //Replace("{Arbitrator.ShortName}", "").
                                    //Replace("{Arbitrator.Mobile}", "").
                                    //Replace("{Arbitrator.Email}", "").
                                    //Replace("{Arbitrator.AreaName}", "").
                                    //Replace("{Arbitrator.CityName}", "").

                                    Replace("{ApplicantCitizen.FullName}", litigationRequest.ApplicantCitizen.FullName).
                                    Replace("{ApplicantCitizen.ShortName}", litigationRequest.ApplicantCitizen.ShortName).
                                    Replace("{ApplicantCitizen.Mobile}", litigationRequest.ApplicantCitizen.Mobile).
                                    Replace("{ApplicantCitizen.Email}", litigationRequest.ApplicantCitizen.Email).
                                    Replace("{ApplicantCitizen.AreaName}", litigationRequest.ApplicantCitizen.Area?.AreaName).
                                    Replace("{ApplicantCitizen.CityName}", litigationRequest.ApplicantCitizen.City?.CityName);
            }
            else
                return "";

            return template;
        }

        private List<string> GetSerializeKeys()
        {
            return new List<string>()
            {
                "Request.RequestCode}",
                "Request.Contract.ContractKey}",
                "Request.AreaName}",
                "Request.CityName}",
                "Request.LitigationCategoryName}",
                "Request.Title}",

                "Plaintiff.Citizen.FullName}",
                "Plaintiff.Citizen.ShortName}",
                "Plaintiff.Citizen.Mobile}",
                "Plaintiff.Citizen.Email}",
                "Plaintiff.Citizen.AreaName}",
                "Plaintiff.Citizen.CityName}",

                "Plaintiff.Facility.ArabicFacilityName}",
                "Plaintiff.Facility.Facility700Number}",
                "Plaintiff.Facility.CommercialRegisterNumber}",

                "Defendant.Citizen.FullName}",
                "Defendant.Citizen.ShortName}",
                "Defendant.Citizen.Mobile}",
                "Defendant.Citizen.Email}",
                "Defendant.Citizen.AreaName}",
                "Defendant.Citizen.CityName}",

                "Defendant.Facility.ArabicFacilityName}",
                "Defendant.Facility.Facility700Number}",
                "Defendant.Facility.CommercialRegisterNumber}",

                "DepartmentAdmin.FullName}",
                "DepartmentAdmin.ShortName}",
                "DepartmentAdmin.Mobile}",
                "DepartmentAdmin.Email}",
                "DepartmentAdmin.AreaName}",
                "DepartmentAdmin.CityName}",

                "Reformer.FullName}",
                "Reformer.ShortName}",
                "Reformer.Mobile}",
                "Reformer.Email}",
                "Reformer.AreaName}",
                "Reformer.CityName}",

                "Arbitrator.FullName}",
                "Arbitrator.ShortName}",
                "Arbitrator.Mobile}",
                "Arbitrator.Email}",
                "Arbitrator.AreaName}",
                "Arbitrator.CityName}",

                "ApplicantCitizen.FullName}",
                "ApplicantCitizen.ShortName}",
                "ApplicantCitizen.Mobile}",
                "ApplicantCitizen.Email}",
                "ApplicantCitizen.AreaName}",
                "ApplicantCitizen.CityName}",


            };
        }
    }

}
