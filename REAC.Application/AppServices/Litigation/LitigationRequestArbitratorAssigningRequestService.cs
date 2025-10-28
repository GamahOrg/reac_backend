using AutoMapper;
using Core.Contracts;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.AppServices.System;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace REAC.Application.AppServices.Litigation
{

    public class LitigationRequestArbitratorAssigningRequestService : BasicService<LitigationRequestArbitratorAssigningRequest, Guid>, ILitigationRequestArbitratorAssigningRequestService
    {
        private readonly REACDBContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtReader _jwtReader;
        private readonly ICitizenService _citizen;
        private readonly ISysEmployeeService _employeeService;
        private readonly INotificationService _notificationService;
        private readonly ISysSettingService _sysSettingService;

        public LitigationRequestArbitratorAssigningRequestService(REACDBContext context, IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ICitizenService citizen, ISysSettingService sysSetting, IMailService emailService, ISMSService smsService, ISysEmployeeService employeeService, ISysSettingService sysSettingService, INotificationService notificationService)
            : base(repository, mapper, loggerManager, jwtReader)
        {
            _context = context;
            _mapper = mapper;
            _jwtReader = jwtReader;
            _citizen = citizen;
            _employeeService = employeeService;
            _sysSettingService = sysSettingService;
            _notificationService = notificationService;
        }
        public async Task<BaseResponse> AddLitigationRequestArbitratorAssigningRequestsResponse(AssigningArbitratorRequestsPayload requestPayload, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
               .Where(c => c.Id == referenceId)
               .FirstOrDefault();

            if (citizenInfo?.SigntureFileId == null)
            {
                return new BaseResponse() { IsDone = false, Message = "يرجى تسجيل التوقيع الالكتروني الخاص بك" };
            }

            var litigationRequestArbitratorAssigningRequest = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())
                         .Include(l => l.ArbitratorCitizen)
                         .FirstOrDefault(x => x.Id == requestPayload.Id);

            var litigationRequestPendingArbitratorAssigningRequest = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())
                         .Include(l => l.ArbitratorCitizen)
                         .Where(x => x.LitigationRequestId == litigationRequestArbitratorAssigningRequest.LitigationRequestId && x.Response == LitigationRequestArbitratorAssigningRequestStatus.None).AsNoTracking().ToList();

            var litigationRequestAcceptedArbitratorAssigningRequest = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())
                         .Include(l => l.ArbitratorCitizen)
                         .Where(x => x.LitigationRequestId == litigationRequestArbitratorAssigningRequest.LitigationRequestId && x.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve).AsNoTracking().ToList();

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestArbitratorNominationLists)
                        .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                        .ThenInclude(x => x.ArbitratorCitizen)
                          .Include(x => x.Contract)
                        .ThenInclude(x => x.FirstPartyCitizen)
                              .Include(x => x.Contract)
                        .ThenInclude(x => x.SecondPartyCitizen)
                           .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                        .ThenInclude(x => x.ArbitratorChairmanCitizen)
                        .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen).ThenInclude(x => x.LitigationRequestArbitratorLists)
                             .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen).ThenInclude(x => x.ArbitratorLitigationRequests)
                        .Include(x => x.LitigationRequestArbitratorLists)
                        .Where(x => x.Id == litigationRequestArbitratorAssigningRequest.LitigationRequestId)
                        .SingleOrDefault();


            if (litigationRequestArbitratorAssigningRequest != null)
            {
                litigationRequestArbitratorAssigningRequest.ResponseOn = DateTime.Now;
                litigationRequestArbitratorAssigningRequest.Response = requestPayload.LitigationRequestReformerAssigningRequestStatus;
                if (!string.IsNullOrEmpty(requestPayload.RejectReason))
                {
                    litigationRequestArbitratorAssigningRequest.RejectReason = requestPayload.RejectReason;

                }
            }

            if (litigationRequest != null)
            {

                var msg = "";
                string body;
                var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);

                var applicationNotifications = new List<SysNotificationDto>();

                switch (requestPayload.LitigationRequestReformerAssigningRequestStatus)
                {
                    case LitigationRequestArbitratorAssigningRequestStatus.Approve:
                        switch (litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId)
                        {
                            case ArbitratorAssigningRequestType.Arbitrator:
                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = "تم قبول المهمة من المحكم " + litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName
                                });

                                if (litigationRequest.LitigationRequestArbitratorLists != null)
                                {
                                    if (litigationRequest.LitigationRequestArbitratorLists.Count(x => x.ArbitratorCitizenId == referenceId) == 0)
                                    {
                                        litigationRequest.LitigationRequestArbitratorLists.Add(new LitigationRequestArbitratorList()
                                        {
                                            Id = Guid.NewGuid(),
                                            ArbitratorCitizenId = referenceId,
                                            LitigationRequestId = litigationRequest.Id
                                        });
                                    }
                                }
                                else
                                {
                                    litigationRequest.LitigationRequestArbitratorLists.Add(new LitigationRequestArbitratorList()
                                    {
                                        Id = Guid.NewGuid(),
                                        ArbitratorCitizenId = referenceId,
                                        LitigationRequestId = litigationRequest.Id
                                    });
                                }

                                if (litigationRequest.ArbitratorsCount == 1)
                                {
                                    litigationRequest.ArbitratorCitizenId = referenceId;
                                    litigationRequest.LitigationRequestStage = LitigationRequestStage.Arbitration;
                                    litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAddArbitrationSession;
                                    litigationRequest.StartArbitrationDate = DateTime.Now;
                                    litigationRequest.ExternalStatusId = RequestStatus.WaitingForAsessionTimeSelection;
                                    litigationRequest.InternalStatusId = RequestStatus.WaitingForAsessionTimeSelection;
                                }

                                if (litigationRequestPendingArbitratorAssigningRequest.Count == 1 &&
                                    (litigationRequestAcceptedArbitratorAssigningRequest.Count == litigationRequest.ArbitratorsCount - 2 || litigationRequest.ArbitratorsCount == 1) &&
                                    (litigationRequest.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestAddArbitratorListNominationDone) )
                                {
                                    litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAddArbitratorChairmanListNomination;

                                    //TODO: Assign new list contains remaining of arbitrator nomination list to chairman List 
                                    //Get Arbitration Citizen nomination List 
                                    var NominationRequestListIds = litigationRequest.LitigationRequestArbitratorNominationLists.Where(s => s.LitigationRequestArbitratorAssigningRequests.Any(x => x.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve)).Select(x => x.ArbitratorCitizenId).ToList();
                                    var NominationList = litigationRequest.LitigationRequestArbitratorNominationLists.Where(s => !NominationRequestListIds.Contains(s.ArbitratorCitizenId)).ToList();
                                    NominationList.ForEach(async t =>
                                    {
                                        LitigationRequestArbitratorsChairmanNominationList litigationRequestArbitratorNominationList = new LitigationRequestArbitratorsChairmanNominationList();
                                        litigationRequestArbitratorNominationList.ArbitratorChairmanCitizenId = t.ArbitratorCitizenId;
                                        litigationRequestArbitratorNominationList.LitigationRequestId = litigationRequest.Id;
                                        litigationRequestArbitratorNominationList.Id = Guid.NewGuid();
                                        await _repository.AddAsync<LitigationRequestArbitratorsChairmanNominationList, Guid>(litigationRequestArbitratorNominationList);
                                    });
                                    litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAddArbitratorChairmanListNominationDone;
                                    litigationRequest.ArbitratorsChairmanNominationListCreationDateTime = DateTime.Now;
                                    //reset flag for  cancellation arbitration chairman SLA 
                                    litigationRequest.IsArbitratorReminderForCancelArbitrationSent = false;
                                    //var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                                    //if (!repositorySaveResponse.IsDone)
                                    //    return repositorySaveResponse;

                                    // Send Notification
                                    // تم إضافة قائمة مرشحى رئيس هيئة التحكيم 
                                    try
                                    {
                                        if (deptAdminInfoArbitration.IsDone)
                                        {
                                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_Added).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_Added).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }

                                            List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                                                      new SysNotificationDto{
                                                   ActorId = (Guid)deptAdminInfoArbitration.Data?.Id,
                                                   RequestId=litigationRequest.Id,
                                                   CreatedOn= DateTime.Now,
                                                   RequestTypeId=RequestType.LitigationRequest,
                                                   ActorTypeId=NotificationActorType.Employee,
                                                   Message=msg
                                                   }
                                                  
                                                   };
                                         List<string> emails = new List<string>() {
                                            deptAdminInfoArbitration.Data?.Email
                                           };

                                            foreach (var arbitrator in litigationRequest.LitigationRequestArbitratorLists)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {

                                                    ActorId = arbitrator.ArbitratorCitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Arbitrator,
                                                    Message = msg
                                                });
                                                emails.Add(arbitrator.ArbitratorCitizen.Email);
                                            }
                                            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                                            {
                                                To = emails,
                                                Subject = "تم إضافة قائمة مرشحى رئيس هيئة التحكيم ",
                                                Body = body
                                            }, null, notifications);
                                        }

                                    }
                                    catch (Exception e)
                                    {


                                    }
                                }
                                break;
                            case ArbitratorAssigningRequestType.ArbitratorsChairman:
                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = "تم قبول المهمة من المحكم " + litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName + " كرئيس هيئة التحكيم"
                                });

                                litigationRequest.ArbitratorCitizenId = referenceId;
                                litigationRequest.LitigationRequestStage = LitigationRequestStage.Arbitration;
                                litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAddArbitrationSession;
                                litigationRequest.StartArbitrationDate = DateTime.Now;
                                litigationRequest.ExternalStatusId = RequestStatus.WaitingForAsessionTimeSelection;
                                litigationRequest.InternalStatusId = RequestStatus.WaitingForAsessionTimeSelection;
                                break;
                        }

                        // Send Notification
                        // تم قبول طلب المهمة من المحكم
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_AcceptTask).Result.Data?.Value;
                            msg = msg.Replace("{ArbitratorName}", litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName);
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_AcceptTask).Result.Data?.Value);
                                body = body.Replace("{ArbitratorName}", litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = citizenInfo.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Arbitrator,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = litigationRequestArbitratorAssigningRequest.Id,
                                RequestTypeId = RequestType.ArbitratorTaskrequests,
                                Message = msg
                            });
                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = deptAdminInfoArbitration.Data.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Employee,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = litigationRequestArbitratorAssigningRequest.Id,
                                RequestTypeId = RequestType.ArbitratorTaskrequests,
                                Message = msg
                            });

                            await _notificationService.SendNotifications(
                               new SendEmailDto()
                               {
                                   To = new List<string>() { citizenInfo.Email, deptAdminInfoArbitration.Data.Email },
                                   Subject = "Arbitraor Accept Task",
                                   Body = body
                               },
                               new SMSRequestDto()
                               {
                                   message_text = msg,
                                   recipients = new List<string>() { citizenInfo.Mobile, deptAdminInfoArbitration.Data.Mobile },
                                   notification_type = "SMS",

                               },
                               applicationNotifications
                               );
                        }
                        catch (Exception ex)
                        {

                        }

                        break;
                    case LitigationRequestArbitratorAssigningRequestStatus.Reject:
                        switch (litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId)
                        {
                            case ArbitratorAssigningRequestType.Arbitrator:

                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = "تم رفض المهمة من المحكم " + litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName
                                });
                                //ToDo
                                //implement new flow for single arbitrator
                                switch (litigationRequest.ArbitratorsCount)
                                {
                                    case null:
                                        break;
                                    case 0:
                                        break;
                                    case 1:
                                        var arbitratorAssigned = litigationRequest.LitigationRequestArbitratorNominationLists.FirstOrDefault(s => s.ArbitratorCitizenId == referenceId);
                                        if (arbitratorAssigned != null)
                                            arbitratorAssigned.IsResponse = true;

                                        var arbitratorsList = litigationRequest.LitigationRequestArbitratorNominationLists.Where(s => s.IsSent != true && s.IsSelectedFromManager != true).OrderBy(x => x.TotalPriority).ThenBy(x => x.DefendantPriority).ToList();
                                        await ProcessArbitratorSelectionForSingle(litigationRequest, arbitratorsList);
                                        break;
                                    default:
                                        //make reselect again from plintiff or defendant 
                                        //send notification to him for reselect 
                                        //update arbitrator response
                                        var arbitratorsAssigned = litigationRequest.LitigationRequestArbitratorNominationLists.FirstOrDefault(s => s.ArbitratorCitizenId == referenceId);
                                        if (arbitratorsAssigned != null)
                                            arbitratorsAssigned.IsResponse = true;
                                        //update litigation request for plaintiff or deffendant to reassign
                                        if (arbitratorsAssigned?.IsPlaintiffSelected == true)
                                        {
                                            litigationRequest.IsPlaintiffArbitratorNominationListDone = false;
                                        }
                                        else if (arbitratorsAssigned?.IsDefendantSelected == true)
                                        {
                                            litigationRequest.IsDefendantArbitratorNominationListDone = false;
                                        }

                                        var ContractCitizen = litigationRequest.IsPlaintiffArbitratorNominationListDone == false ? litigationRequest.Contract.FirstPartyCitizen
                                                          : litigationRequest.Contract.SecondPartyCitizen;
                                        // Send Notification
                                        // تم رفض طلب المهمة من المحكم
                                        try
                                        {
                                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_RejectTask).Result.Data?.Value;
                                            msg = msg.Replace("{ArbitratorName}", litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName);
                                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_RejectTask).Result.Data?.Value);
                                                body = body.Replace("{ArbitratorName}", litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            applicationNotifications.Add(new SysNotificationDto()
                                            {
                                                ActorId = ContractCitizen.Id,
                                                CreatedOn = DateTime.Now,
                                                ActorTypeId = NotificationActorType.Arbitrator,
                                                IsViewed = false,
                                                ViewedOn = null,
                                                RequestId = litigationRequestArbitratorAssigningRequest.Id,
                                                RequestTypeId = RequestType.ArbitratorTaskrequests,
                                                Message = msg
                                            });

                                            applicationNotifications.Add(new SysNotificationDto()
                                            {
                                                ActorId = deptAdminInfoArbitration.Data.Id,
                                                CreatedOn = DateTime.Now,
                                                ActorTypeId = NotificationActorType.Employee,
                                                IsViewed = false,
                                                ViewedOn = null,
                                                RequestId = litigationRequestArbitratorAssigningRequest.Id,
                                                RequestTypeId = RequestType.ArbitratorTaskrequests,
                                                Message = msg
                                            });

                                            await _notificationService.SendNotifications(
                                               new SendEmailDto()
                                               {
                                                   To = new List<string>() { ContractCitizen.Email, deptAdminInfoArbitration.Data.Email },
                                                   Subject = "Arbitraor Reject Task",
                                                   Body = body
                                               },
                                               new SMSRequestDto()
                                               {
                                                   message_text = msg,
                                                   recipients = new List<string>() { ContractCitizen.Mobile, deptAdminInfoArbitration.Data.Mobile },
                                                   notification_type = "SMS",

                                               },
                                               applicationNotifications
                                               );
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                        // Send Notification
                                        // يرجى اعادة تعيين محكم جديد للدعوة رقم 
                                        try
                                        {

                                            msg = $"يتطلب القرار اليدوي لاعادة تعيين محكم للدعوى رقم {litigationRequest.RequestCode}";
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_RejectTask).Result.Data?.Value);
                                                body = body.Replace("{ArbitratorName}", litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            applicationNotifications.Add(new SysNotificationDto()
                                            {
                                                ActorId = ContractCitizen.Id,
                                                CreatedOn = DateTime.Now,
                                                ActorTypeId = NotificationActorType.Arbitrator,
                                                IsViewed = false,
                                                ViewedOn = null,
                                                RequestId = litigationRequestArbitratorAssigningRequest.Id,
                                                RequestTypeId = RequestType.ArbitratorTaskrequests,
                                                Message = msg
                                            });

                                            applicationNotifications.Add(new SysNotificationDto()
                                            {
                                                ActorId = deptAdminInfoArbitration.Data.Id,
                                                CreatedOn = DateTime.Now,
                                                ActorTypeId = NotificationActorType.Employee,
                                                IsViewed = false,
                                                ViewedOn = null,
                                                RequestId = litigationRequestArbitratorAssigningRequest.Id,
                                                RequestTypeId = RequestType.ArbitratorTaskrequests,
                                                Message = msg
                                            });

                                            await _notificationService.SendNotifications(
                                               new SendEmailDto()
                                               {
                                                   To = new List<string>() { ContractCitizen.Email, deptAdminInfoArbitration.Data.Email },
                                                   Subject = "Arbitraor Reject Task, Please reassign another one ",
                                                   Body = body
                                               },
                                               new SMSRequestDto()
                                               {
                                                   message_text = msg,
                                                   recipients = new List<string>() { ContractCitizen.Mobile, deptAdminInfoArbitration.Data.Mobile },
                                                   notification_type = "SMS",

                                               },
                                               applicationNotifications
                                               );
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                        //arbitratorsList = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsSent != true).OrderBy(x => x.TotalPriority).ThenBy(x => x.DefendantPriority).ToList();
                                        //if (arbitratorsList != null)
                                        //{
                                        //    if (arbitratorsList.Count > 0)
                                        //    {
                                        //        LitigationRequestArbitratorAssigningRequest nextLitigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                                        //        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                                        //        nextLitigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                                        //        nextLitigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.Arbitrator;
                                        //        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                                        //        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = arbitratorsList.FirstOrDefault().Id;
                                        //        nextLitigationRequestArbitratorAssigningRequest.CitizenId = arbitratorsList.FirstOrDefault().ArbitratorCitizenId;
                                        //        nextLitigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                                        //        await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(nextLitigationRequestArbitratorAssigningRequest);
                                        //        arbitratorsList.FirstOrDefault().IsSent = true;

                                        //        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                        //        {
                                        //            Id = Guid.NewGuid(),
                                        //            CreatedOn = DateTime.Now,
                                        //            RequestId = litigationRequest.Id,
                                        //            ShowForApplicant = false,
                                        //            LogMessage = "تم إرسال طلب مهمة تحكيم للمحكم " + arbitratorsList.FirstOrDefault()?.ArbitratorCitizen.FullName + " كمحكم"
                                        //        });

                                        //        // تم إرسال طلب مهمة تحكيم جديدة كمحكم على الدعوى رقم ..... 
                                        //        try
                                        //        {
                                        //            var msgReassign = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_ReAssign).Result.Data?.Value;
                                        //            msgReassign = msgReassign.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                        //            //var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                        //            //string body2;
                                        //            //using (StreamReader streamReader = File.OpenText(pathToFile))
                                        //            //{
                                        //            //    body2 = await streamReader.ReadToEndAsync();
                                        //            //    body2 = body2?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_ReAssign).Result.Data?.Value);
                                        //            //    body2 = body2?.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                        //            //}
                                        //            //await SendNotificationMS(body2, msgReassign, litigationRequestArbitratorAssigningRequest.LitigationRequest, "لم يتم تعيين رئيس هيئة التحكيم", NotificationActorType.Employee, RequestType.ArbitratorTaskrequests
                                        //            //, notifyDeptAdmin: true);


                                        //            var citizenInfo2 = (await _repository.GetQueryable<Citizen, Guid>())
                                        //                .FirstOrDefault(c => c.Id == nextLitigationRequestArbitratorAssigningRequest.CitizenId);


                                        //            applicationNotifications.Add(new SysNotificationDto()
                                        //            {
                                        //                ActorId = nextLitigationRequestArbitratorAssigningRequest.CitizenId,
                                        //                CreatedOn = DateTime.Now,
                                        //                ActorTypeId = NotificationActorType.Arbitrator,
                                        //                IsViewed = false,
                                        //                ViewedOn = null,
                                        //                RequestId = nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId,
                                        //                RequestTypeId = RequestType.ArbitratorTaskrequests,
                                        //                Message = msgReassign
                                        //            });

                                        //            var notificationResult = await _notificationService.SendNotifications(
                                        //                new SendEmailDto()
                                        //                {
                                        //                    Body = msgReassign,
                                        //                    Subject = "تم إرسال طلب مهمة تحكيم جديدة كمحكم",
                                        //                    To = new List<string>() { citizenInfo2.Email }

                                        //                },
                                        //                new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                        //                {
                                        //                    message_text = msgReassign,
                                        //                    notification_type = "SMS",
                                        //                    recipients = new List<string>() { citizenInfo2.Mobile }
                                        //                }, applicationNotifications
                                        //                );
                                        //        }
                                        //        catch (Exception ex)
                                        //        {

                                        //        }
                                        //    }
                                        //}
                                        break;

                                }
                                break;
                            case ArbitratorAssigningRequestType.ArbitratorsChairman:
                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = "تم رفض المهمة من المحكم " + litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName + " كرئيس هيئة التحكيم"
                                });
                                var chairmanArbitratorsList = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Where(x => x.IsSent != true).OrderBy(x => x.TotalPriority).ToList();
                                if (chairmanArbitratorsList != null)
                                    await ProcessArbitratorChairmanSelectionForSingle(litigationRequest, chairmanArbitratorsList);
                                
                                //if (chairmanArbitratorsList != null)
                                //{
                                //    if (chairmanArbitratorsList.Count > 0)
                                //    {
                                //        LitigationRequestArbitratorAssigningRequest nextLitigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                                //        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                                //        nextLitigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                                //        nextLitigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.ArbitratorsChairman;
                                //        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                                //        //nextLitigationRequestArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = chairmanArbitratorsList.FirstOrDefault().Id;
                                //        nextLitigationRequestArbitratorAssigningRequest.CitizenId = chairmanArbitratorsList.FirstOrDefault().ArbitratorChairmanCitizenId;
                                //        nextLitigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                                //        await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(nextLitigationRequestArbitratorAssigningRequest);
                                //        chairmanArbitratorsList.FirstOrDefault().IsSent = true;

                                //        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                //        {
                                //            Id = Guid.NewGuid(),
                                //            CreatedOn = DateTime.Now,
                                //            RequestId = litigationRequest.Id,
                                //            ShowForApplicant = false,
                                //            LogMessage = "تم إرسال طلب مهمة للمحكم " + chairmanArbitratorsList.FirstOrDefault()?.ArbitratorChairmanCitizen.FullName + " كرئيس هيئة التحكيم"
                                //        });
                                //    }
                                //}

                                //// تم إرسال طلب مهمة تحكيم جديدة كمحكم على الدعوى رقم ..... 
                                //try
                                //{
                                //    var arbitratorInfo = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(c => c.Id == chairmanArbitratorsList.FirstOrDefault().ArbitratorChairmanCitizenId);
                                //    var msgReassign = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmitedToArbitrator).Result.Data?.Value;
                                //    msgReassign = msgReassign.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);

                                //    applicationNotifications.Add(new SysNotificationDto()
                                //    {
                                //        ActorId = chairmanArbitratorsList.FirstOrDefault().ArbitratorChairmanCitizenId,
                                //        CreatedOn = DateTime.Now,
                                //        ActorTypeId = NotificationActorType.Arbitrator,
                                //        IsViewed = false,
                                //        ViewedOn = null,
                                //        RequestId = litigationRequest.Id,
                                //        RequestTypeId = RequestType.ArbitratorTaskrequests,
                                //        Message = msgReassign
                                //    });

                                //    var notificationResult = await _notificationService.SendNotifications(
                                //        new SendEmailDto()
                                //        {
                                //            Body = msgReassign,
                                //            Subject = "تم إرسال طلب مهمة تحكيم جديدة كمحكم",
                                //            To = new List<string>() { arbitratorInfo.Email }

                                //        },
                                //        new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                //        {
                                //            message_text = msgReassign,
                                //            notification_type = "SMS",
                                //            recipients = new List<string>() { arbitratorInfo.Mobile }
                                //        }, applicationNotifications
                                //        );
                                //}
                                //catch (Exception ex)
                                //{

                                //}

                                break;
                        }
                        // Send Notification
                        // تم رفض طلب المهمة من المحكم

                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_RejectTask).Result.Data?.Value;
                            msg = msg.Replace("{ArbitratorName}", litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName);
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_RejectTask).Result.Data?.Value);
                                body = body.Replace("{ArbitratorName}", litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = citizenInfo.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Arbitrator,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = litigationRequestArbitratorAssigningRequest.Id,
                                RequestTypeId = RequestType.ArbitratorTaskrequests,
                                Message = msg
                            });

                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = deptAdminInfoArbitration.Data.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Employee,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = litigationRequestArbitratorAssigningRequest.Id,
                                RequestTypeId = RequestType.ArbitratorTaskrequests,
                                Message = msg
                            });

                            await _notificationService.SendNotifications(
                               new SendEmailDto()
                               {
                                   To = new List<string>() { citizenInfo.Email, deptAdminInfoArbitration.Data.Email },
                                   Subject = "Arbitraor Reject Task",
                                   Body = body
                               },
                               new SMSRequestDto()
                               {
                                   message_text = msg,
                                   recipients = new List<string>() { citizenInfo.Mobile, deptAdminInfoArbitration.Data.Mobile },
                                   notification_type = "SMS",

                               },
                               applicationNotifications
                               );
                        }
                        catch (Exception ex)
                        {

                        }


                        break;
                }
            }
            var saveResponse = await _repository.SaveChangesAsync(cancellationToken);
            if (!saveResponse.IsDone)
                return saveResponse;

            //litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
            //            .Include(x => x.LitigationRequestArbitratorLists)
            //            .Where(x => x.Id == litigationRequestArbitratorAssigningRequest.LitigationRequestId)
            //            .SingleOrDefault();

            //switch (litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId)
            //{
            //    case ArbitratorAssigningRequestType.Arbitrator:
            //        if (litigationRequest.LitigationRequestArbitratorLists.Count == litigationRequest.ArbitratorsCount - 1)
            //            litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAddArbitratorChairmanListNomination;
            //        break;
            //}

            //saveResponse = await _repository.SaveChangesAsync(cancellationToken);
            //if (!saveResponse.IsDone)
            //    return saveResponse;


            return await GetLitigationRequestArbitratorAssigningRequests(litigationRequestArbitratorAssigningRequest.LitigationRequestId, requestPayload.Id, cancellationToken);
        }
        public async Task<BaseResponse> GetLitigationRequestArbitratorAssigningRequests(int pageIndex, int pageSize, CancellationToken cancellationToken, LitigationRequestArbitrationAssignedRequestFilter? filter, bool ascending = false)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            // Check if arbitrator is certified
            if (referenceId != Guid.Empty)
            {
                var arbitratorStatusId = (await _repository.GetQueryable<Citizen, Guid>())
                    .FirstOrDefault(x => x.Id == referenceId)
                    .ArbitratorStatusId;

                if (arbitratorStatusId == ArbitratorStatus.Stopped)
                {
                    return new BaseResponse() { IsDone = false, Message = "تم ايقاف الحساب الخاص بك" };
                }
            }

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.Area)
                         .Include(x => x.City)
                         .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                         .Include(x => x.LitigationRequestExtendPeriodRequests)
                         .ThenInclude(x => x.LitigationRequestExtendPeriodRequestFiles)
                         .ThenInclude(x => x.FileManager)
                         .Include(x => x.ApplicantCitizen)
                         .Include(x => x.InternalStatus)
                         .Include(x => x.ExternalStatus)
                         .Include(x => x.LitigationRequestAmount)
                         .Include(x => x.LitigationRequestCategory)
                         .Include(x => x.SysExemptionReductionCategory)
                         .Include(x => x.LitigationRequestOwnerType)
                         .Include(x => x.Contract)
                         .Include(x => x.Contract.ContractFile)
                         .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                         .Include(x => x.Contract.FoundingLetterFile)
                         .Include(x => x.Contract.InternalRegulationsFile)
                         .Include(x => x.Contract.FirstPartyFacility)
                         .Include(x => x.Contract.FirstPartyCitizen)
                         .Include(x => x.Contract.SecondPartyFacility)
                         .Include(x => x.Contract.SecondPartyCitizen)
                         .Include(x => x.Contract.LitigationRequestContractType)
                         .Where(x => x.LitigationRequestArbitratorAssigningRequests
                         .Where(t => t.CitizenId == referenceId && t.Response != LitigationRequestArbitratorAssigningRequestStatus.Cancelled).Count() > 0)
                         .AsNoTracking();


            if (filter is not null)
            {
                Expression<Func<LitigationRequest, bool>> combinedCondition = PredicateBuilder.True<LitigationRequest>();

                if (filter.LitigationRequestCode is not null)
                {
                    combinedCondition = combinedCondition.AndAlso(x => x.RequestCode.Contains(filter.LitigationRequestCode));
                    litigationRequests = litigationRequests.Where(combinedCondition);

                }

                if (filter.LitigationRequestType is not null)
                {
                    combinedCondition = combinedCondition.AndAlso(x => x.Contract.LitigationRequestContractTypeId == (LitigationRequestContractType)filter.LitigationRequestType);
                    litigationRequests = litigationRequests.Where(combinedCondition);

                }

                if (filter.RequestStatus is not null)
                {
                    litigationRequests = litigationRequests.Where(x => x.LitigationRequestArbitratorAssigningRequests.Any(t => t.CitizenId == referenceId && t.Response == (LitigationRequestArbitratorAssigningRequestStatus)filter.RequestStatus));

                }

                if (filter.PlaintiffName is not null)
                {
                    combinedCondition = combinedCondition.AndAlso(x => x.Contract.FirstPartyCitizen.FullName.Contains(filter.PlaintiffName) || x.Contract.FirstPartyFacility.ArabicFacilityName.Contains(filter.PlaintiffName));
                    litigationRequests = litigationRequests.Where(combinedCondition);

                }

                if (filter.DefendantName is not null)
                {
                    combinedCondition = combinedCondition.AndAlso(x => x.Contract.SecondPartyCitizen.FullName.Contains(filter.PlaintiffName) || x.Contract.SecondPartyFacility.ArabicFacilityName.Contains(filter.PlaintiffName));
                    litigationRequests = litigationRequests.Where(combinedCondition);

                }




            }


            var result = ascending
                ? litigationRequests.OrderBy(x => x.SentOn).GetPaged(pageIndex, pageSize)
                : litigationRequests.OrderByDescending(x => x.SentOn).GetPaged(pageIndex, pageSize);

            List<CustomLitigationRequestMircoArbitratorDto> litigationRequestMircoDtos = new List<CustomLitigationRequestMircoArbitratorDto>();
            if (result != null)
            {
                foreach (var litigationRequest in result.Results)
                {
                    foreach (var litigationRequestArbitratorAssigningRequest in litigationRequest.LitigationRequestArbitratorAssigningRequests
                        .Where(x => x.CitizenId == referenceId).DistinctBy(x => new { x.LitigationRequestId,x.ArbitratorAssigningRequestTypeId}))
                    {
                        int arbitrationPeriod = 0;
                        int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Period).Result.Data?.Value, out arbitrationPeriod);



                        int daysPassed = (DateTime.Now - litigationRequestArbitratorAssigningRequest.RequestOn).Days;
                        litigationRequestArbitratorAssigningRequest.LitigationRequest.ArbitrationPeriod = Math.Max(0, arbitrationPeriod - daysPassed);


                        CustomLitigationRequestMircoArbitratorDto litigationRequestMircoDto = new CustomLitigationRequestMircoArbitratorDto();
                        litigationRequestMircoDto.Id = litigationRequestArbitratorAssigningRequest.Id;
                        litigationRequestMircoDto.RequestId = litigationRequest.Id;
                        litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                        litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                        litigationRequestMircoDto.ArbitrationPeriod = litigationRequestArbitratorAssigningRequest.LitigationRequest.ArbitrationPeriod;
                        litigationRequestMircoDto.ArbitratorAssigningRequestType = litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId;
                        litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                        litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                        litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                        litigationRequestMircoDto.Response = litigationRequestArbitratorAssigningRequest.Response;
                        litigationRequestMircoDto.ResponseOn = litigationRequestArbitratorAssigningRequest.ResponseOn;
                        litigationRequestMircoDto.ArbitratorAssigningRequestDate = litigationRequestArbitratorAssigningRequest.RequestOn;
                        litigationRequestMircoDto.ArbitrationFinishedExpectedDate = DateTime.Now.AddDays((long)litigationRequestArbitratorAssigningRequest.LitigationRequest.ArbitrationPeriod).ToLocalTime();

                        bool allowAdd = true;

                        if (litigationRequest.LitigationRequestArbitratorAssigningRequests.Where(x => x.CitizenId == referenceId).Any())
                        {
                            if (litigationRequestArbitratorAssigningRequest.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve |
                                litigationRequestArbitratorAssigningRequest.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled |
                                litigationRequestArbitratorAssigningRequest.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject)
                            {
                                allowAdd = false;
                            }
                        }

                        if (allowAdd)
                            litigationRequestMircoDto.IsDone = "--";
                        else
                            litigationRequestMircoDto.IsDone = "تم";

                        litigationRequestMircoDto.Status = litigationRequest.ExternalStatus.RequestStatus;
                        litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                    }
                }
            }

            return new DataResponse<PagedResult<CustomLitigationRequestMircoArbitratorDto>>
            {
                Data =
                              new PagedResult<CustomLitigationRequestMircoArbitratorDto>
                              {
                                  Results = litigationRequestMircoDtos,
                                  CurrentPage = result.CurrentPage,
                                  PageCount = result.PageCount,
                                  PageSize = result.PageSize,
                                  TotalCount = result.TotalCount
                              },
                IsDone = true
            };


        }
        public async Task<BaseResponse> GetLitigationRequestArbitratorAssigningRequestsAll(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequestArbitratorAssigningRequest> litigationRequestArbitratorAssigningRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())
                         .Include(x => x.ArbitratorCitizen)
                         .Include(x => x.LitigationRequest)
                         .Include(x => x.LitigationRequest.InternalStatus)
                         .Include(x => x.LitigationRequest.ExternalStatus)
                         .Include(x => x.LitigationRequest.LitigationRequestCategory)
                         .Include(x => x.LitigationRequest.Contract)
                         .Include(x => x.LitigationRequest.Contract.ContractFile)
                         .Include(x => x.LitigationRequest.Contract.AssociationRegistrationCertificateFile)
                         .Include(x => x.LitigationRequest.Contract.FoundingLetterFile)
                         .Include(x => x.LitigationRequest.Contract.InternalRegulationsFile)
                         .Include(x => x.LitigationRequest.Contract.FirstPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.FirstPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.LitigationRequestContractType);


            litigationRequestArbitratorAssigningRequests = entitiesQuerable.Where(x => x.LitigationRequest.LitigationRequestStage
            == LitigationRequestStage.Arbitration)
                                       .OrderByDescending(x => x.LitigationRequest.SentOn)
                                       .AsSplitQuery()
                                       .AsNoTracking()
                                       .GetPaged(pageIndex, pageSize);



            List<CustomLitigationRequestMircoArbitratorAllDto> litigationRequestMircoDtos = new List<CustomLitigationRequestMircoArbitratorAllDto>();
            if (litigationRequestArbitratorAssigningRequests != null)
            {
                int arbitrationPeriod = 0;
                int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Period).Result.Data?.Value, out arbitrationPeriod);


                foreach (var litigationRequestArbitratorAssigningRequest in litigationRequestArbitratorAssigningRequests.Results)
                {
                    int daysPassed = (DateTime.Now - litigationRequestArbitratorAssigningRequest.RequestOn).Days;
                    litigationRequestArbitratorAssigningRequest.LitigationRequest.ArbitrationPeriod = Math.Max(0, arbitrationPeriod - daysPassed);
                }
                foreach (var litigationRequest in litigationRequestArbitratorAssigningRequests.Results)
                {
                    CustomLitigationRequestMircoArbitratorAllDto litigationRequestMircoDto = new CustomLitigationRequestMircoArbitratorAllDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestId = litigationRequest.LitigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.LitigationRequest.SentOn != null ? (DateTime)litigationRequest.LitigationRequest.SentOn : litigationRequest.LitigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.LitigationRequest.RequestCode;
                    litigationRequestMircoDto.ArbitrationPeriod = litigationRequest.LitigationRequest.ArbitrationPeriod;
                    litigationRequestMircoDto.ArbitratorAssigningRequestType = litigationRequest.ArbitratorAssigningRequestTypeId;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.LitigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.LitigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.LitigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.LitigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.LitigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.LitigationRequest.Contract.SecondPartyCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.LitigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.RequestTypeId = litigationRequest.LitigationRequest.Contract.LitigationRequestContractType.Id.GetHashCode();
                    litigationRequestMircoDto.Response = litigationRequest.Response;
                    litigationRequestMircoDto.ResponseOn = litigationRequest.ResponseOn;
                    litigationRequestMircoDto.ArbitratorName =
                        (litigationRequest.ArbitratorAssigningRequestTypeId ==
                        ArbitratorAssigningRequestType.ArbitratorsChairman) ? litigationRequest.ArbitratorCitizen?.FullName :
                        litigationRequest.ArbitratorCitizen?.FullName;

                    bool allowAdd = true;


                    if (litigationRequest.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve |
                        litigationRequest.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled |
                        litigationRequest.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject)
                    {
                        allowAdd = false;
                    }


                    if (allowAdd)
                        litigationRequestMircoDto.IsDone = "--";
                    else
                        litigationRequestMircoDto.IsDone = "تم";

                    litigationRequestMircoDto.Status = litigationRequest.LitigationRequest.ExternalStatus.RequestStatus;
                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                }
            }

            return new DataResponse<PagedResult<CustomLitigationRequestMircoArbitratorAllDto>>
            {
                Data =
                              new PagedResult<CustomLitigationRequestMircoArbitratorAllDto>
                              {
                                  Results = litigationRequestMircoDtos,
                                  CurrentPage = litigationRequestArbitratorAssigningRequests.CurrentPage,
                                  PageCount = litigationRequestArbitratorAssigningRequests.PageCount,
                                  PageSize = litigationRequestArbitratorAssigningRequests.PageSize,
                                  TotalCount = litigationRequestArbitratorAssigningRequests.TotalCount
                              },
                IsDone = true
            };
        }
        public async Task<BaseResponse> GetLitigationRequestArbitratorAssigningRequests(Guid requestId, Guid LitigationRequestArbitratorAssigningRequestId, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.Area)
                         .Include(x => x.City)
                         .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                         .Include(x => x.ApplicantCitizen)
                         .Include(x => x.InternalStatus)
                         .Include(x => x.ExternalStatus)
                         .Include(x => x.LitigationRequestAmount)
                         .Include(x => x.LitigationRequestCategory)
                         .Include(x => x.SysExemptionReductionCategory)
                         .Include(x => x.LitigationRequestOwnerType)
                         .Include(x => x.Contract)
                         .Include(x => x.Contract.ContractFile)
                         .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                         .Include(x => x.Contract.FoundingLetterFile)
                         .Include(x => x.Contract.InternalRegulationsFile)
                         .Include(x => x.Contract.FirstPartyFacility)
                         .Include(x => x.Contract.FirstPartyCitizen)
                         .Include(x => x.Contract.SecondPartyFacility)
                         .Include(x => x.Contract.SecondPartyCitizen)
                         .Include(x => x.Contract.LitigationRequestContractType)
                         .Where(x => x.LitigationRequestArbitratorAssigningRequests
                         .Where(t => t.Id == LitigationRequestArbitratorAssigningRequestId).Count() > 0).OrderByDescending(x => x.CreatedOn).AsNoTracking().ToList().FirstOrDefault();

            if (litigationRequest != null)
            {
                CustomLitigationRequestMircoArbitratorDto litigationRequestMircoDto = new CustomLitigationRequestMircoArbitratorDto();

                litigationRequestMircoDto.Id = litigationRequest.Id;
                litigationRequestMircoDto.RequestId = requestId;
                litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                litigationRequestMircoDto.ArbitratorAssigningRequestType = litigationRequest?.LitigationRequestArbitratorAssigningRequests.FirstOrDefault()?.ArbitratorAssigningRequestTypeId;
                litigationRequestMircoDto.RequestCode = litigationRequest?.RequestCode;
                litigationRequestMircoDto.FirstPartyName = (litigationRequest?.Contract.FirstPartyCitizen == null) ? litigationRequest?.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                litigationRequestMircoDto.SecondPartyName = (litigationRequest?.Contract.SecondPartyCitizen == null) ? litigationRequest?.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                litigationRequestMircoDto.RequestTypeName = litigationRequest?.Contract.LitigationRequestContractType.TypeName;
                litigationRequestMircoDto.Response = litigationRequest.LitigationRequestArbitratorAssigningRequests.FirstOrDefault()?.Response;
                litigationRequestMircoDto.ResponseOn = litigationRequest.LitigationRequestArbitratorAssigningRequests.FirstOrDefault()?.ResponseOn;

                bool allowAdd = true;

                if (litigationRequest.LitigationRequestArbitratorAssigningRequests.Any())
                {
                    if (litigationRequest.LitigationRequestArbitratorAssigningRequests.FirstOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve |
                        litigationRequest.LitigationRequestArbitratorAssigningRequests.FirstOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled |
                        litigationRequest.LitigationRequestArbitratorAssigningRequests.FirstOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject)
                    {
                        allowAdd = false;
                    }
                }

                if (allowAdd)
                    litigationRequestMircoDto.IsDone = "--";
                else
                    litigationRequestMircoDto.IsDone = "تم";

                litigationRequestMircoDto.Status = litigationRequest.InternalStatus.RequestStatus;

                return new DataResponse<CustomLitigationRequestMircoArbitratorDto>()
                {
                    Data = litigationRequestMircoDto,
                    IsDone = true
                };

            }
            throw new NotImplementedException();
        }

        private async Task SendNotificationMS(string mailMsg, string msg, LitigationRequest litigationRequest, string subject, NotificationActorType notificationActorType, RequestType requestType, LitigationRequestArbitratorList? arbitrator = null, bool notifyApplicant = false, bool notifyArbitrators = false, bool notifyDeptAdmin = false, bool notifyFirstParty = false, bool notifySecondParty = false, bool notifyArbitratorChairman = false)
        {
            var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
            var objEmail = new SendEmailDto()
            {
                Body = mailMsg,
                Subject = subject,
                To = new List<string>()
            };

            var objSms = new Contracts.Dtos.Integration.GEE.SMSRequestDto()
            {
                message_text = msg,
                recipients = new List<string>(),
                notification_type = "SMS",
            };

            var lstNotification = new List<SysNotificationDto>();

            // Notify Applicant Citizen
            if (notifyApplicant && litigationRequest.ApplicantCitizen != null)
            {
                objEmail.To.Add(litigationRequest.ApplicantCitizen.Email);
                objSms.recipients.Add(litigationRequest.ApplicantCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.ApplicantCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Arbitrator Citizen
            if (notifyArbitrators && arbitrator?.ArbitratorCitizen is not null)
            {
                objEmail.To.Add(arbitrator.ArbitratorCitizen.Email);
                objSms.recipients.Add(arbitrator.ArbitratorCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = arbitrator.ArbitratorCitizenId,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Department Admin
            if (notifyDeptAdmin && deptAdminInfoArbitration != null)
            {
                objEmail.To.Add(deptAdminInfoArbitration.Data.Email);
                objSms.recipients.Add(deptAdminInfoArbitration.Data.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = deptAdminInfoArbitration.Data.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Chairman 
            if (notifyArbitratorChairman && litigationRequest.ArbitratorCitizen != null)
            {
                objEmail.To.Add(litigationRequest.ArbitratorCitizen.Email);
                objSms.recipients.Add(litigationRequest.ArbitratorCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.ArbitratorCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify First Party Citizen
            if (notifyFirstParty && litigationRequest.Contract?.FirstPartyCitizen != null && litigationRequest.Contract.FirstPartyCitizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest.Contract.FirstPartyCitizen.Email);
                objSms.recipients.Add(litigationRequest.Contract.FirstPartyCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.FirstPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }
            // Notify First Party Facility
            //if (notifyFirstParty && litigationRequest.Contract?.FirstPartyFacility != null && litigationRequest.Contract.FirstPartyFacility.Citizen.Id != litigationRequest.ApplicantCitizen.Id)
            //{
            //    objEmail.To.Add(litigationRequest.Contract.FirstPartyFacility.Email);
            //    objSms.recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
            //    lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
            //    {
            //        ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.Citizen.Id,
            //        CreatedOn = DateTime.Now,
            //        ActorTypeId = notificationActorType,
            //        IsViewed = false,
            //        ViewedOn = null,
            //        RequestId = litigationRequest.Id,
            //        RequestTypeId = requestType,
            //        Message = msg,
            //    });
            //}



            // Notify Second Party Citizen
            if (notifySecondParty && litigationRequest.Contract?.SecondPartyCitizen != null)
            {
                objEmail.To.Add(litigationRequest.Contract.SecondPartyCitizen.Email);
                objSms.recipients.Add(litigationRequest.Contract.SecondPartyCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.SecondPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }
            // Notify Second Party Facility
            //if (notifySecondParty && litigationRequest.Contract?.SecondPartyFacility != null)
            //{
            //    objEmail.To.Add(litigationRequest.Contract.SecondPartyFacility.Email);
            //    objSms.recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);
            //    lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
            //    {
            //        ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.Citizen.Id,
            //        CreatedOn = DateTime.Now,
            //        ActorTypeId = notificationActorType,
            //        IsViewed = false,
            //        ViewedOn = null,
            //        RequestId = litigationRequest.Id,
            //        RequestTypeId = requestType,
            //        Message = msg,
            //    });
            //}

            await _notificationService.SendNotifications(objEmail, objSms, lstNotification);
        }


        #region Extention method for Add Single Nomination Assigning Arbitrator
        public async Task<bool> ProcessArbitratorSelectionForSingle(LitigationRequest litigationRequest, List<LitigationRequestArbitratorNominationList>? arbitratorsList)
        {
            // Calculate total priority scores for each arbitrator
            var arbitratorScores = new List<ArbitratorSelectionScore>();

            foreach (var arbitrator in arbitratorsList)
            {
                var score = new ArbitratorSelectionScore
                {
                    Arbitrator = arbitrator,
                    TotalScore = arbitrator.TotalPriority,
                    ExperienceYears = arbitrator.ArbitratorCitizen?.CitizenRequests?
                                                                                    .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                                                                    .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears ?? 0,
                    RequestsCount = arbitrator.ArbitratorCitizen?.ArbitratorLitigationRequests.Count() + arbitrator.ArbitratorCitizen?.LitigationRequestArbitratorLists.Count,
                    GeographicalPriority = await CompareGeographicalDistance(arbitrator.ArbitratorCitizen, litigationRequest)
                };
                arbitratorScores.Add(score);
            }

            // Apply score & weight calculation criteria
            var selectedArbitrator = await ApplySelectionCriteria(litigationRequest, arbitratorsList, arbitratorScores);

            if (selectedArbitrator != null)
            {
                await AddSingleNominationAssigningArbitrator(litigationRequest, selectedArbitrator);
                return true;
            }

            return true;
        }

        private async Task<LitigationRequestArbitratorNominationList> ApplySelectionCriteria(LitigationRequest litigationRequest, List<LitigationRequestArbitratorNominationList> arbitratorsList, List<ArbitratorSelectionScore> arbitratorScores)
        {

            // Step 1: Find arbitrators with minimum total score
            var minScore = arbitratorScores.Min(x => x.TotalScore);
            var candidatesWithMinScore = arbitratorScores.Where(x => x.TotalScore == minScore).ToList();

            if (candidatesWithMinScore.Count == 1)
            {
                return candidatesWithMinScore.First().Arbitrator;
            }

            // Step 2: Apply experience tiebreaker (highest experience wins)
            var maxExperience = candidatesWithMinScore.Max(x => x.ExperienceYears);
            var experienceCandidates = candidatesWithMinScore.Where(x => x.ExperienceYears == maxExperience).ToList();

            if (experienceCandidates.Count == 1)
            {
                return experienceCandidates.First().Arbitrator;
            }

            // Step 3: Apply case count tiebreaker (lowest case count wins)
            var minCaseCount = experienceCandidates.Min(x => x.RequestsCount);
            var caseCountCandidates = experienceCandidates.Where(x => x.RequestsCount == minCaseCount).ToList();

            if (caseCountCandidates.Count == 1)
            {
                return caseCountCandidates.First().Arbitrator;
            }

            // Step4: Apply geographical distance tiebreaker (closest area wins)
            var bestGeographicalPriority = caseCountCandidates.Min(x => x.GeographicalPriority);
            var geographicalCandidates = caseCountCandidates.Where(x => x.GeographicalPriority == bestGeographicalPriority).ToList();

            if (geographicalCandidates.Count == 1)
            {
                return geographicalCandidates.First().Arbitrator;
            }

            // If still tied, return null to escalate to manager
            // Escalate to arbitration office manager
            await EscalateToManager(litigationRequest, arbitratorsList, geographicalCandidates);
            return null;
        }
        private async Task<decimal> CompareGeographicalDistance(Citizen arbitratorCitizen, LitigationRequest litigationRequest)
        {
            var arbitratorCitizenRequest = arbitratorCitizen.CitizenRequests?.Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                                                                       .OrderBy(x => x.RequestOn).LastOrDefault();

            if (arbitratorCitizenRequest == null) return int.MaxValue;

            if (litigationRequest.AreaId == arbitratorCitizenRequest.AreaId)
            {
                return 0;
            }
            else
            {
                var areaDistances = (await _repository.GetQueryable<SysAreaDistances, Guid>())
                                  .Where(s => s.SourceAreaId == litigationRequest.AreaId && s.DestinationAreaId == arbitratorCitizenRequest.AreaId).FirstOrDefault();
                if (areaDistances != null)
                {
                    return areaDistances.Distance;
                }
                return int.MaxValue;
            }
            //// Priority: 0 = same area, 1 = different area
            //if (arbitratorCitizenRequest.AreaId == litigationRequest.AreaId)
            //{
            //    return 0; 
            //}
            //else
            //{
            //    return 1;
            //}
        }

        public async Task AddSingleNominationAssigningArbitrator(LitigationRequest litigationRequest, LitigationRequestArbitratorNominationList selectedArbitrator)
        {

            LitigationRequestArbitratorAssigningRequest litigationRequestOneArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
            litigationRequestOneArbitratorAssigningRequest.RequestOn = DateTime.Now;
            litigationRequestOneArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.Arbitrator;
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = selectedArbitrator.Id;
            litigationRequestOneArbitratorAssigningRequest.CitizenId = selectedArbitrator.ArbitratorCitizenId;
            litigationRequestOneArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
            await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(litigationRequestOneArbitratorAssigningRequest);
            selectedArbitrator.IsSent = true;
            // Send Notification
            // يوجد طلب مهمه جديده كمحكم علي الدعوي رقم
            try
            {
                var Fmsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_ReAssign).Result.Data?.Value;
                Fmsg = Fmsg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                string body;
                using (StreamReader streamReader = File.OpenText(pathToFile))
                {
                    body = await streamReader.ReadToEndAsync();
                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_ReAssign).Result.Data?.Value);
                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                }
                var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                                    .Where(c => c.Id == litigationRequestOneArbitratorAssigningRequest.CitizenId)
                                    .FirstOrDefault();


                List<SysNotificationDto> notificationList = new List<SysNotificationDto>() {

                           new SysNotificationDto{
                        ActorId = litigationRequestOneArbitratorAssigningRequest.CitizenId,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.ArbitratorTaskrequests,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=Fmsg
                        }

                        };
                List<string> emailList = new List<string>() {
                                  citizenInfo.Email
                                };

                List<string> recipientList = new List<string>()
                            {
                                  citizenInfo.Mobile
                            };

                await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                {
                    To = emailList,
                    Subject = "طلب مهمة جديدة كمحكم علي الدعوي",
                    Body = body
                }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                {
                    message_text = Fmsg,
                    recipients = recipientList,
                    notification_type = "SMS",

                }, notificationList);

            }
            catch (Exception e)
            {


            }


        }

        // Escalate to arbitration office manager
        private async Task EscalateToManager(LitigationRequest litigationRequest, List<LitigationRequestArbitratorNominationList> tiedArbitrators, List<ArbitratorSelectionScore> arbitratorScores)
        {
            // Find arbitrators with minimum total score
            //var minScore = arbitratorScores.Min(x => x.TotalScore);
            tiedArbitrators = arbitratorScores.Select(x => x.Arbitrator).ToList();

            foreach (var arbitrator in tiedArbitrators)
            {

                // Reset the sent status for escalation
                arbitrator.IsEscalated = true;
                arbitrator.EscalateDate = DateTime.Now;
                //arbitrator.IsSelectedFromManager = false;
            }

            // Notify arbitration office manager
            // Send Notification
            // يتطلب القرار اليدوي لاختيار المحكم للدعوى
            try
            {
                var msg = $"يتطلب القرار اليدوي لاختيار المحكم للدعوى رقم {litigationRequest.RequestCode} بتاريخ{DateTime.Now.ToLongDateString()}";
                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                string body;
                using (StreamReader streamReader = File.OpenText(pathToFile))
                {
                    body = await streamReader.ReadToEndAsync();
                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsNominationList_AssignedAndSent).Result.Data?.Value);
                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                }
                var arbitrationAdmin = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                if (arbitrationAdmin.IsDone)
                {
                    List<SysNotificationDto> notificationList = new List<SysNotificationDto>() {

                           new SysNotificationDto{
                        ActorId = (Guid)arbitrationAdmin.Data?.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Employee,
                        Message=msg
                        }

                        };
                    List<string> emailList = new List<string>() {
                                  arbitrationAdmin.Data?.Email
                                };

                    List<string> recipientList = new List<string>()
                            {
                                  arbitrationAdmin.Data?.Mobile
                            };

                    await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                    {
                        To = emailList,
                        Subject = "يتطلب القرار اليدوي لاختيار المحكم للدعوى رقم",
                        Body = body
                    }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                    {
                        message_text = msg,
                        recipients = recipientList,
                        notification_type = "SMS",

                    }, notificationList);
                }
            }
            catch (Exception ex)
            {


            }

        }

        #endregion



        #region Extention method for Add Single Nomination Chairman Assigning Arbitrator
        public async Task<bool> ProcessArbitratorChairmanSelectionForSingle(LitigationRequest litigationRequest, List<LitigationRequestArbitratorsChairmanNominationList>? arbitratorsList)
        {
            // Calculate total priority scores for each arbitrator
            var arbitratorScores = new List<ArbitratorChairmanSelectionScore>();

            foreach (var arbitrator in arbitratorsList)
            {
                var score = new ArbitratorChairmanSelectionScore
                {
                    Arbitrator = arbitrator,
                    TotalScore = arbitrator.TotalPriority,
                    ExperienceYears = arbitrator.ArbitratorChairmanCitizen?.CitizenRequests?
                                                                                    .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                                                                    .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears ?? 0,
                    RequestsCount = arbitrator.ArbitratorChairmanCitizen?.ArbitratorLitigationRequests.Count() + arbitrator.ArbitratorChairmanCitizen?.LitigationRequestArbitratorLists.Count,
                    GeographicalPriority = await CompareGeographicalDistanceChairman(arbitrator.ArbitratorChairmanCitizen, litigationRequest)
                };
                arbitratorScores.Add(score);
            }

            // Apply score & weight calculation criteria
            var selectedArbitrator = await ApplySelectionCriteriaChairman(litigationRequest, arbitratorsList, arbitratorScores);

            if (selectedArbitrator != null)
            {
                await AddSingleNominationChairmanAssigningArbitrator(litigationRequest, selectedArbitrator);
                return true;
            }

            return true;
        }

        private async Task<LitigationRequestArbitratorsChairmanNominationList> ApplySelectionCriteriaChairman(LitigationRequest litigationRequest, List<LitigationRequestArbitratorsChairmanNominationList> arbitratorsList, List<ArbitratorChairmanSelectionScore> arbitratorScores)
        {

            // Step 1: Find arbitrators with minimum total score
            var minScore = arbitratorScores.Min(x => x.TotalScore);
            var candidatesWithMinScore = arbitratorScores.Where(x => x.TotalScore == minScore).ToList();

            if (candidatesWithMinScore.Count == 1)
            {
                return candidatesWithMinScore.First().Arbitrator;
            }

            // Step 2: Apply experience tiebreaker (highest experience wins)
            var maxExperience = candidatesWithMinScore.Max(x => x.ExperienceYears);
            var experienceCandidates = candidatesWithMinScore.Where(x => x.ExperienceYears == maxExperience).ToList();

            if (experienceCandidates.Count == 1)
            {
                return experienceCandidates.First().Arbitrator;
            }

            // Step 3: Apply case count tiebreaker (lowest case count wins)
            var minCaseCount = experienceCandidates.Min(x => x.RequestsCount);
            var caseCountCandidates = experienceCandidates.Where(x => x.RequestsCount == minCaseCount).ToList();

            if (caseCountCandidates.Count == 1)
            {
                return caseCountCandidates.First().Arbitrator;
            }

            // Step4: Apply geographical distance tiebreaker (closest area wins)
            var bestGeographicalPriority = caseCountCandidates.Min(x => x.GeographicalPriority);
            var geographicalCandidates = caseCountCandidates.Where(x => x.GeographicalPriority == bestGeographicalPriority).ToList();

            if (geographicalCandidates.Count == 1)
            {
                return geographicalCandidates.First().Arbitrator;
            }

            // If still tied, return null to escalate to manager
            // Escalate to arbitration office manager
            await EscalateToManagerChairman(litigationRequest, arbitratorsList, geographicalCandidates);
            return null;
        }
        private async Task<decimal> CompareGeographicalDistanceChairman(Citizen arbitratorCitizen, LitigationRequest litigationRequest)
        {
            var arbitratorCitizenRequest = arbitratorCitizen.CitizenRequests?.Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                                                                       .OrderBy(x => x.RequestOn).LastOrDefault();

            if (arbitratorCitizenRequest == null) return int.MaxValue;

            if (litigationRequest.AreaId == arbitratorCitizenRequest.AreaId)
            {
                return 0;
            }
            else
            {
                var areaDistances = (await _repository.GetQueryable<SysAreaDistances, Guid>())
                                  .Where(s => s.SourceAreaId == litigationRequest.AreaId && s.DestinationAreaId == arbitratorCitizenRequest.AreaId).FirstOrDefault();
                if (areaDistances != null)
                {
                    return areaDistances.Distance;
                }
                return int.MaxValue;
            }

            // Priority: 0 = same area, 1 = different area
            //if (arbitratorCitizenRequest.AreaId == litigationRequest.AreaId)
            //{
            //    return 0; // Same area - highest priority
            //}
            //else
            //{
            //    return 1; // Different area - lowest priority
            //}
        }

        public async Task AddSingleNominationChairmanAssigningArbitrator(LitigationRequest litigationRequest, LitigationRequestArbitratorsChairmanNominationList selectedArbitrator)
        {

            LitigationRequestArbitratorAssigningRequest litigationRequestOneArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
            litigationRequestOneArbitratorAssigningRequest.RequestOn = DateTime.Now;
            litigationRequestOneArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.ArbitratorsChairman;
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = litigationRequest.LitigationRequestArbitratorAssigningRequests.FirstOrDefault(s => s.CitizenId == selectedArbitrator.ArbitratorChairmanCitizenId)?.LitigationRequestArbitratorNominationListId ?? null;
            litigationRequestOneArbitratorAssigningRequest.CitizenId = selectedArbitrator.ArbitratorChairmanCitizenId;
            litigationRequestOneArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
            await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(litigationRequestOneArbitratorAssigningRequest);
            selectedArbitrator.IsSent = true;

            // Send Notification
            //  يوجد طلب مهمه جديد كرئيس هية التحكيم لدعوي رقم

            try
            {

                var arbitratorInfo = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(c => c.Id == litigationRequestOneArbitratorAssigningRequest.CitizenId);


                var msg = "";
                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmitedToArbitrator).Result.Data?.Value;
                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                string body;
                using (StreamReader streamReader = File.OpenText(pathToFile))
                {
                    body = await streamReader.ReadToEndAsync();
                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmitedToArbitrator).Result.Data?.Value);
                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                }

                List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                        new SysNotificationDto{
                        ActorId = arbitratorInfo.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=msg
                        },


                        };
                await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                {
                    To = new List<string>() { arbitratorInfo.Email },
                    Subject = "طلب مهمة جديدة",
                    Body = body
                }, null, notifications);

            }
            catch (Exception e)
            {


            }


        }

        // Escalate to arbitration office manager
        private async Task EscalateToManagerChairman(LitigationRequest litigationRequest, List<LitigationRequestArbitratorsChairmanNominationList> tiedArbitrators, List<ArbitratorChairmanSelectionScore> arbitratorScores)
        {
            // Find arbitrators with minimum total score
            //var minScore = arbitratorScores.Min(x => x.TotalScore);
            //tiedArbitrators = tiedArbitrators.Where(x => x.TotalPriority == minScore).ToList();
            tiedArbitrators = arbitratorScores.Select(x => x.Arbitrator).ToList();

            foreach (var arbitrator in tiedArbitrators)
            {

                // Reset the sent status for escalation
                arbitrator.IsEscalated = true;
                arbitrator.EscalateDate = DateTime.Now;
                //arbitrator.IsSelectedFromManager = false;
            }
            litigationRequest.IsArbitratorsChairmanDecisionRequired = true;
            // Notify arbitration office manager
            // Send Notification
            // يتطلب القرار اليدوي لاختيار المحكم للدعوى
            try
            {
                var msg = $"يتطلب القرار اليدوي لاعادة اختيار رئيس هيئه التحكيم  للدعوى رقم {litigationRequest.RequestCode}";
                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                string body;
                using (StreamReader streamReader = File.OpenText(pathToFile))
                {
                    body = await streamReader.ReadToEndAsync();
                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_MakeDecision).Result.Data?.Value);
                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                }
                var arbitrationAdmin = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);

                if (arbitrationAdmin.IsDone)
                {
                    List<SysNotificationDto> notificationList = new List<SysNotificationDto>() {

                         new SysNotificationDto()
                            {
                                ActorId = arbitrationAdmin.Data.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Employee,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = litigationRequest.Id,
                                RequestTypeId = RequestType.ArbitratorTaskrequests,
                                Message = msg
                            }

                }
                ;
                    List<string> emailList = new List<string>() {
                                  arbitrationAdmin.Data?.Email
                                };

                    List<string> recipientList = new List<string>()
                            {
                                  arbitrationAdmin.Data?.Mobile
                            };

                    await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                    {
                        To = emailList,
                        Subject = "يتطلب القرار اليدوي لاختيار رئيس هيئه التحكيم للدعوى رقم",
                        Body = body
                    }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                    {
                        message_text = msg,
                        recipients = recipientList,
                        notification_type = "SMS",

                    }, notificationList);
                }
            }
            catch (Exception ex)
            {


            }

        }

        #endregion
    }
}
