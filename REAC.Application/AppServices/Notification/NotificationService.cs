using AutoMapper;
using Core.Contracts;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Common;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.EntityFrameworkCore.Entities.Notification;
using System.Text.RegularExpressions;
using Wafi.Application.Hubs;
using Wafi.Application.Hubs.Contracts;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace REAC.Application.AppServices.Notification
{
    public class NotificationService : BasicService<SysNotification, Guid>, INotificationService
    {
        private readonly IJsonStringLocalizer _localization;
        private readonly IJwtReader _jwtReader;
        protected readonly IMailService _emailService;
        protected readonly ISMSService _smsService;
        protected readonly IHubContext<NotificationHub, INotificationClient> _notificationHub;

        public NotificationService(IEntityRepository repository,
        IMapper mapper,
            ILoggerManager loggerManager,
            IJsonStringLocalizer localization,
            IJwtReader jwtReader,
            IMailService emailService,
            ISMSService smsService,
            IHubContext<NotificationHub, INotificationClient> notificationHub) : base(repository, mapper, loggerManager, jwtReader)
        {
            _localization = localization;
            _jwtReader = jwtReader;
            _emailService = emailService;
            _smsService = smsService;
            _notificationHub = notificationHub;
        }

        public async Task<BaseResponse> GetNotificationListForPortal()
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var query = await _repository.GetQueryable<SysNotification, Guid>();

            query = query.Where(x => x.ActorId == referenceId);

            var notifications = await query.Include(x => x.SysNotificationActorType)
                .Include(x => x.SysRequestType).OrderByDescending(x => x.CreatedOn).Take(20).ToListAsync();
            var notViewNotificationCount = await query.CountAsync(x => x.IsViewed == false);

            return new DataResponse<NotificationListDto>
            {
                IsDone = true,
                Data = new NotificationListDto
                {
                    NotificationList = _mapper.Map<List<NotificationDto>>(notifications).OrderByDescending(x => x.CreatedOn).ToList(),
                    NotViewedCount = notViewNotificationCount,
                }
            };
        }
        public async Task<BaseResponse> GetNotificationListForBackOffice()
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var query = await _repository.GetQueryable<SysNotification, Guid>();

            query = query.Where(x => x.ActorId == referenceId);

            var notifications = await query.Include(x => x.SysNotificationActorType)
                .Include(x => x.SysRequestType).OrderByDescending(x => x.CreatedOn).Take(20).ToListAsync();
            var notViewNotificationCount = await query.CountAsync(x => x.IsViewed == false);


            var response = _mapper.Map<List<NotificationDto>>(notifications).OrderByDescending(x => x.CreatedOn).ToList();


            return new DataResponse<NotificationListDto>
            {
                IsDone = true,
                Data = new NotificationListDto
                {
                    NotificationList = response,
                    NotViewedCount = notViewNotificationCount,
                }
            };
        }
        public async Task<BaseResponse> MarkNotificationViewed(Guid notificationId)
        {
            try
            {
                if (notificationId != Guid.Empty)
                {
                    var notification = (await _repository.GetQueryable<SysNotification, Guid>()).FirstOrDefault(x => x.Id == notificationId);
                    if (notification == null)
                        return new BaseResponse { IsDone = false, Message = _localization["GeneralNotFound"] };

                    notification.IsViewed = true;
                    notification.ViewedOn = DateTime.Now;
                    _repository.Update<SysNotification, Guid>(notification);
                    return await _repository.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return new BaseResponse { IsDone = false, Message = e.Message + e.InnerException };
            }

            return new BaseResponse { IsDone = false, Message = _localization["GeneralNotFound"] };

        }
        public async Task<BaseResponse> GetNotificationListForPortal(int pageIndex, int pageSize)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<SysNotification> entities = null;

            var entitiesQuerable = (await _repository.GetQueryable<SysNotification, Guid>())
                                   .Include(x => x.SysNotificationActorType)
                                   .Include(x => x.SysRequestType);

            var notViewNotificationCount = await entitiesQuerable.CountAsync(x => x.ActorId == referenceId & x.IsViewed == false);


            entities = entitiesQuerable.Where(x => x.ActorId == referenceId)
                                       .OrderByDescending(x => x.CreatedOn)
                                       .AsSplitQuery()
                                       .AsNoTracking()
                                       .GetPaged(pageIndex, pageSize);

            return new DataResponse<PagedResult<NotificationDto>>
            {
                Data =
                    new PagedResult<NotificationDto>
                    {
                        Results = _mapper.Map<List<NotificationDto>>(entities.Results),
                        CurrentPage = entities.CurrentPage,
                        PageCount = entities.PageCount,
                        PageSize = entities.PageSize,
                        TotalCount = notViewNotificationCount
                    },
                IsDone = true
            };

        }
        public async Task<BaseResponse> GetNotificationListForBackOffice(int pageIndex, int pageSize)
        {

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<SysNotification> entities = null;

            var entitiesQuerable = (await _repository.GetQueryable<SysNotification, Guid>())
                                   .Include(x => x.SysNotificationActorType)
                                   .Include(x => x.SysRequestType);
            var notViewNotificationCount = await entitiesQuerable.CountAsync(x => x.ActorId == referenceId & x.IsViewed == false);


            entities = entitiesQuerable.Where(x => x.ActorId == referenceId)
                                       .OrderByDescending(x => x.CreatedOn)
                                       .AsSplitQuery()
                                       .AsNoTracking()
                                       .GetPaged(pageIndex, pageSize);


            return new DataResponse<PagedResult<NotificationDto>>
            {
                Data =
                    new PagedResult<NotificationDto>
                    {
                        Results = _mapper.Map<List<NotificationDto>>(entities.Results),
                        CurrentPage = entities.CurrentPage,
                        PageCount = entities.PageCount,
                        PageSize = entities.PageSize,
                        TotalCount = notViewNotificationCount
                    },
                IsDone = true
            };

        }

        public async Task<BaseResponse> SendNotifications(SendEmailDto email, SMSRequestDto sms, List<SysNotificationDto> notifications)
        {
            try
            {
                List<SysNotificationDto> newNotificationsList = new List<SysNotificationDto>();
                foreach (var notification in notifications)
                {
                    var isExist = (await _repository.GetQueryable<SysNotification, Guid>())
                        .Any(x => x.ActorId == notification.ActorId
                        && x.RequestId == notification.RequestId
                        && (int)x.NotificationActorTypeId == (int)notification.ActorTypeId
                        && (int)x.RequestTypeId == (int)notification.RequestTypeId
                        && x.Message == notification.Message
                        && x.CreatedOn.Day == DateTime.Today.Day
                        );

                    if (!isExist)
                        newNotificationsList.Add(notification);
                }

                if (email is not null)
                {
                    await _emailService.SendMailToSLA(email.To, email.Subject, email.Body);
                }

                if (sms is not null)
                {
                    var recipiants = new List<string>();
                    foreach (var mobile in sms.recipients)
                    {
                        if (Regex.IsMatch(mobile, "^05[0-9]*$"))
                            recipiants.Add(mobile);
                    }

                    sms.recipients = recipiants;
                    //var smsResultCitizen = await _smsService.SendSMS(sms);
                     await _smsService.SendSMSToSLA(sms);
                }

                if (newNotificationsList.Any())
                {
                    foreach (var notification in newNotificationsList)
                    {
                        switch (notification.ActorTypeId)
                        {
                            case Domain.Shared.Enum.NotificationActorType.Employee:
                                //var notificationResultEmployee = _notificationHub.Clients.All.NotifyEmployee(notification);
                                break;


                            default:
                                //var notificationResultCitizen = _notificationHub.Clients.All.NotifyCitizen(notification);

                                break;
                        }
                        notification.Id = Guid.NewGuid();

                        var notificationObject = _mapper.Map<SysNotification>(notification);
                        notificationObject.NotificationActorTypeId = notification.ActorTypeId;

                        var dbNotificationResult = await _repository.AddAsync<SysNotification, Guid>(notificationObject);


                    }

                    var res = await _repository.SaveChangesAsync();
                }

                return new BaseResponse() { IsDone = true };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = ex.Message };
            }
        }
        
        public async Task<BaseResponse> SendNotificationsWithoutDuplicationChecking(SendEmailDto email, SMSRequestDto sms, List<SysNotificationDto> notifications)
        {
            try
            {
               
                if (email is not null)
                {
                    await _emailService.SendMailToSLA(email.To, email.Subject, email.Body);
                }

                if (sms is not null)
                {
                    var recipiants = new List<string>();
                    foreach (var mobile in sms.recipients)
                    {
                        if (Regex.IsMatch(mobile, "^05[0-9]*$"))
                            recipiants.Add(mobile);
                    }

                    sms.recipients = recipiants;
                    //var smsResultCitizen = await _smsService.SendSMS(sms);
                     await _smsService.SendSMSToSLA(sms);
                }

                if (notifications.Any())
                {
                    foreach (var notification in notifications)
                    {
                        switch (notification.ActorTypeId)
                        {
                            case Domain.Shared.Enum.NotificationActorType.Employee:
                                //var notificationResultEmployee = _notificationHub.Clients.All.NotifyEmployee(notification);
                                break;


                            default:
                                //var notificationResultCitizen = _notificationHub.Clients.All.NotifyCitizen(notification);

                                break;
                        }
                        notification.Id = Guid.NewGuid();

                        var notificationObject = _mapper.Map<SysNotification>(notification);
                        notificationObject.NotificationActorTypeId = notification.ActorTypeId;

                        var dbNotificationResult = await _repository.AddAsync<SysNotification, Guid>(notificationObject);


                    }

                    var res = await _repository.SaveChangesAsync();
                }

                return new BaseResponse() { IsDone = true };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = ex.Message };
            }
        }
    }
}
