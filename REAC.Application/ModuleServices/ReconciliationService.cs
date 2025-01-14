using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IAppServices.UserManage;
using REAC.Application.Contracts.IAppServices.Wpf.Business;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;


namespace REAC.Application.ModuleServices
{
    public partial class ReconciliationService : IReconciliationService
    {
        private readonly IMapper _mapper;
        private readonly IEntityRepository _repository;
        private readonly IJwtReader _jwtReader;
        private readonly ISysEmployeeService _employeeService;
        private readonly INotificationService _notificationService;
        private readonly ISysSettingService _sysSettingService;
        private readonly IZoomService _zoomService;
        private readonly INafathService _nafathService;

        public ReconciliationService(IEntityRepository repository,
                          ISysSettingService sysSettingService,
                          IWorkFlowProcessingLogService workFlowProcessingLogService,
                          IMapper mapper,
                          IJwtReader jwtReader,
                          INotificationService notificationService,
                          ISysEmployeeService employeeService,
                          IZoomService zoomService,
                          INafathService nafathService)

        {
            _mapper = mapper;
            _repository = repository;
            _jwtReader = jwtReader;
            _notificationService = notificationService;
            _sysSettingService = sysSettingService;
            _employeeService = employeeService;
            _zoomService = zoomService;
            _nafathService = nafathService;
        }

        public async Task<BaseResponse> Assign(AssignRequestPayload assignRequestPayload, CancellationToken cancellationToken)
        {
            var msg = "";
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Where(x => x.Id == assignRequestPayload.RequestId).ToList().FirstOrDefault();
            var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                if (referenceId == Guid.Empty)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "موظف غير معروف"
                    };
                var employee = (await _repository.GetQueryable<SysEmployee, Guid>())?
                        .Where(x => x.Id == Guid.Parse("D4430015-C883-413D-B57A-29B5FC6C07C8")).SingleOrDefault();

                if (employee == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "موظف غير معروف"
                    };

                if (!employee.AllowAssign)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "موظف غير مسموح له"
                    };

                litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                      .Where(x => x.Id == assignRequestPayload.RequestId).Single();

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب دعوى غير موجود"
                    };


                if (litigationRequest.IsFinished.HasValue)
                {
                    if (litigationRequest.IsFinished.Value)
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "دعوى مغلقة"
                        };
                }

                if (litigationRequest.LitigationRequestStage != LitigationRequestStage.Reconciliation)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "الدعوى ليست بمرحلة الصلح"
                    };

                if (litigationRequest.ReformerCitizenId.HasValue)
                {
                    if (!employee.AllowReAssign)
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "موظف غير مسموح له"
                        };
                }
                var litigationRequestReformerAssigningRequests = (await _repository.GetQueryable<LitigationRequestReformerAssigningRequest, Guid>())?
                     .Where(x => x.LitigationRequestId == assignRequestPayload.RequestId & x.CitizenId == assignRequestPayload.ReformerId & x.Response == 0);

                if (litigationRequestReformerAssigningRequests.Any())
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "يوجد طلب مهمه سابق لم يتم الرد عليه حتى الان من قبل المصلح"
                    };
                }

                litigationRequestReformerAssigningRequests = (await _repository.GetQueryable<LitigationRequestReformerAssigningRequest, Guid>()).Include(x => x.ReformerCitizen)?


                   .Where(x => x.LitigationRequestId == assignRequestPayload.RequestId & x.Response == 0);

                if (litigationRequestReformerAssigningRequests.Any())
                {

                    foreach (var request in litigationRequestReformerAssigningRequests.ToList())
                    {

                        request.Response = LitigationRequestReformerAssigningRequestStatus.Cancelled;
                        var saveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                        if (!saveResponse.IsDone)
                            return saveResponse;

                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم إلغاء تعيين المصلح " + request.ReformerCitizen?.FullName
                        });
                        // Send Notification
                        // تم إلغاء المهمة (دعوى صلح)
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Cancelled).Result.Data?.Value;
                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);

                            await _notificationService.SendNotifications(
                            new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = new List<string>() { litigationRequest.ReformerCitizen.Email, deptAdminInfoReconciliation.Data.Email },
                                Subject = "Reconciliation cancelled",
                                Body = msg
                            },
                            new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = new List<string>() { litigationRequest.ReformerCitizen.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                                notification_type = "SMS",
                            },
                            new List<SysNotificationDto>()
                            {
                               new SysNotificationDto()
                               {
                                    ActorId = litigationRequest.ReformerCitizen.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Reformer,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = request.Id,
                                    RequestTypeId = RequestType.ReformerTaskrequests,
                                    Message = msg,
                               },
                               new SysNotificationDto()
                               {
                                    ActorId = deptAdminInfoReconciliation.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = request.Id,
                                    RequestTypeId = RequestType.ReformerTaskrequests,
                                    Message = msg,
                               }
                            }
                            );
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }

                var citizen = (await _repository.GetQueryable<Citizen, Guid>()).Where(x => x.Id == assignRequestPayload.ReformerId).FirstOrDefault();


                LitigationRequestReformerAssigningRequest litigationRequestReformerAssigningRequest = new LitigationRequestReformerAssigningRequest();
                litigationRequestReformerAssigningRequest.CitizenId = assignRequestPayload.ReformerId;
                litigationRequestReformerAssigningRequest.LitigationRequestId = litigationRequest.Id;
                litigationRequestReformerAssigningRequest.RequestOn = DateTime.Now;
                litigationRequestReformerAssigningRequest.Response = LitigationRequestReformerAssigningRequestStatus.None;
                await _repository.AddAsync<LitigationRequestReformerAssigningRequest, Guid>(litigationRequestReformerAssigningRequest);

                // Send Notification
                // تم إرسال مهمة مصلح لمصلح أخر
                try
                {
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_ReAssigned).Result.Data?.Value;
                    msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                    msg = msg?.Replace("{ReformerName}", citizen?.FullName);

                    await _notificationService.SendNotifications(
                           new Contracts.Dtos.Notification.SendEmailDto()
                           {
                               To = new List<string>() { deptAdminInfoReconciliation.Data.Email },
                               Subject = "Reconciliation ReAssigned",
                               Body = msg
                           },
                           new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                           {
                               message_text = msg,
                               recipients = new List<string>() { deptAdminInfoReconciliation.Data.Mobile },
                               notification_type = "SMS",
                           },
                           new List<SysNotificationDto>()
                           {

                               new SysNotificationDto()
                               {
                                    ActorId = deptAdminInfoReconciliation.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = litigationRequestReformerAssigningRequest.Id,
                                    RequestTypeId = RequestType.ReformerTaskrequests,
                                    Message = msg,
                               }
                           }
                           );
                }
                catch (Exception ex)
                {

                }


                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    RequestId = litigationRequest.Id,
                    ShowForApplicant = false,
                    LogMessage = "تم إرسال طلب مهمة  للمصلح " + citizen.FullName
                });

                var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                return repositorySaveResponse;
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    Exception = ex.Message + "\n" + ex.ToString(),
                };
            }
        }

        #region Decision
        public async Task<BaseResponse> AddLitigationRequestDecision(LitigationRequestAction requestPayload, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);
            var msg = "";
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.ReformerCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Where(x => x.Id == requestPayload.RequestId).ToList().FirstOrDefault();

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                      .Where(x => x.Id == requestPayload.RequestId & x.ReformerCitizenId == referenceId).ToList();

            if (!litigationRequests.Any())
                return new BaseResponse() { IsDone = false, Message = "طلب الدعوى غير موجود" };

            switch (requestPayload.LitigationRequestDecision)
            {

                case LitigationRequestDecision.WaiverOfRight:
                    litigationRequests.FirstOrDefault().InternalStatusId = RequestStatus.WaiverOfRight;
                    litigationRequests.FirstOrDefault().ExternalStatusId = RequestStatus.WaiverOfRight;
                    litigationRequests.FirstOrDefault().IsActive = false;
                    litigationRequests.FirstOrDefault().IsFinished = true;
                    litigationRequests.FirstOrDefault().LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestReformerDecisionAdded;
                    // Send Notification
                    // تم التنازل عن الحق
                    try
                    {
                        msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_WaiverOfRight).Result.Data?.Value;
                        msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                        SendNotificationMSWithAllTypes(msg, litigationRequest, "Citizen Litigation Request For Report Reply Added");
                    }
                    catch (Exception e)
                    { }
                    break;
                case LitigationRequestDecision.ReconciliationWasMade:
                    litigationRequests.FirstOrDefault().InternalStatusId = RequestStatus.ReconciliationWasMade;
                    litigationRequests.FirstOrDefault().ExternalStatusId = RequestStatus.ReconciliationWasMade;
                    litigationRequests.FirstOrDefault().IsActive = false;
                    litigationRequests.FirstOrDefault().IsFinished = true;
                    litigationRequests.FirstOrDefault().LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestReformerDecisionAdded;
                    // Send Notification
                    // تم الصلح
                    try
                    {
                        msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_ReconciliationWasMade).Result.Data?.Value;
                        msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                        SendNotificationMSWithAllTypes(msg, litigationRequest, "Citizen Litigation Request For Report Reply Added");
                    }
                    catch (Exception e)
                    { }
                    break;
                case LitigationRequestDecision.WaiverOfTheLitigation:
                    litigationRequests.FirstOrDefault().InternalStatusId = RequestStatus.WaiverOfTheLitigation;
                    litigationRequests.FirstOrDefault().ExternalStatusId = RequestStatus.WaiverOfTheLitigation;
                    litigationRequests.FirstOrDefault().IsActive = false;
                    litigationRequests.FirstOrDefault().IsFinished = true;
                    litigationRequests.FirstOrDefault().LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestReformerDecisionAdded;
                    // Send Notification
                    // تم التنازل عن الدعوى
                    try
                    {
                        msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_WaiverOfTheLitigation).Result.Data?.Value;
                        msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                        SendNotificationMSWithAllTypes(msg, litigationRequest, "Decision of reconciliation was added");
                    }
                    catch (Exception e)
                    { }
                    break;
                case LitigationRequestDecision.ReconciliationWasNotMade:
                    if (litigationRequests.FirstOrDefault().LitigationRequestCategoryId == LitigationRequestCategory.ReconciliationAndArbitration)
                    {
                        litigationRequests.FirstOrDefault().InternalStatusId = RequestStatus.ReconciliationWasNotMadeAndMoveToArbitration;
                        litigationRequests.FirstOrDefault().ExternalStatusId = RequestStatus.ReconciliationWasNotMadeAndMoveToArbitration;
                        litigationRequests.FirstOrDefault().LitigationRequestStage = LitigationRequestStage.Arbitration;
                        litigationRequests.FirstOrDefault().LitigationRequestSubStage = LitigationRequestSubStage.ArbitratorSelectionPeriod;
                        // Send Notification
                        // عدم الصلح والانتقال للتحكيم
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_ReconciliationWasNotMadeAndMoveToArbitration).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            SendNotificationMSWithAllTypes(msg, litigationRequest, "Citizen Litigation Request For Report Reply Added");
                        }
                        catch (Exception e)
                        {


                        }
                    }
                    else
                    {
                        litigationRequests.FirstOrDefault().InternalStatusId = RequestStatus.ReconciliationWasNotMade;
                        litigationRequests.FirstOrDefault().ExternalStatusId = RequestStatus.ReconciliationWasNotMade;
                        litigationRequests.FirstOrDefault().IsActive = false;
                        litigationRequests.FirstOrDefault().IsFinished = true;

                        // Send Notification
                        // عدم الصلح

                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_ReconciliationWasNotMade).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            await SendNotificationMSWithAllTypes(msg, litigationRequest, "Citizen Litigation Request For Report Reply Added");
                        }
                        catch (Exception e)
                        { }
                    }
                    break;
            }

            litigationRequests.FirstOrDefault().EndReconciliationDate = DateTime.Now;

            var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            if (!repositorySaveResponse.IsDone)
                return repositorySaveResponse;

            return await GetLitigationRequestForReport(requestPayload.RequestId, cancellationToken);

        }
        #endregion
    }
}
