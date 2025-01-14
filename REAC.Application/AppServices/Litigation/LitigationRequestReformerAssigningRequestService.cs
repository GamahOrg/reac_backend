using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.AppServices.Litigation
{

    public class LitigationRequestReformerAssigningRequestService : BasicService<LitigationRequestReformerAssigningRequest, Guid>, ILitigationRequestReformerAssigningRequest
    {
        private readonly REACDBContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtReader _jwtReader;
        private readonly ICitizenService _citizen;
        private readonly ISysEmployeeService _employeeService;
        private readonly INotificationService _notificationService;
        private readonly ISysSettingService _sysSettingService;

        public LitigationRequestReformerAssigningRequestService(REACDBContext context, IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ICitizenService citizen, ISysEmployeeService employeeService, INotificationService notificationService, ISysSettingService sysSettingService) : base(repository, mapper, loggerManager, jwtReader)
        {
            _context = context;
            _mapper = mapper;
            _jwtReader = jwtReader;
            _citizen = citizen;
            _employeeService = employeeService;
            _notificationService = notificationService;
            _sysSettingService = sysSettingService;
        }
        public async Task<BaseResponse> AddLitigationRequestReformerAssigningRequestsResponse(AssigningRequestsPayload requestPayload, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequestReformerAssigningRequest = (await _repository.GetQueryable<LitigationRequestReformerAssigningRequest, Guid>())
                            .Include(l => l.ReformerCitizen)
                         .Where(x => x.Id == requestPayload.Id).ToList().FirstOrDefault();

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestExtendPeriodRequests)
                        .Where(x => x.Id == litigationRequestReformerAssigningRequest.LitigationRequestId)
                        .SingleOrDefault();


            if (litigationRequestReformerAssigningRequest != null)
            {
                litigationRequestReformerAssigningRequest.ResponseOn = DateTime.Now;
                litigationRequestReformerAssigningRequest.Response = requestPayload.LitigationRequestReformerAssigningRequestStatus;
                if (!string.IsNullOrEmpty(requestPayload.RejectReason))
                    litigationRequestReformerAssigningRequest.RejectReason = requestPayload.RejectReason;
            }

            if (litigationRequest != null)
            {
                var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                    .Where(c => c.Id == referenceId)
                    .FirstOrDefault();
                var msg = "";
                var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);

                var applicationNotifications = new List<SysNotificationDto>();

                switch (requestPayload.LitigationRequestReformerAssigningRequestStatus)
                {
                    case LitigationRequestReformerAssigningRequestStatus.Approve:
                        litigationRequest.ReformerCitizenId = referenceId;
                        litigationRequest.LitigationRequestStage = LitigationRequestStage.Reconciliation;
                        litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.AddSession;
                        litigationRequest.StartReconciliationDate = DateTime.Now;

                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم فبول الدعوى من قبل المصلح " + litigationRequestReformerAssigningRequest.ReformerCitizen.FullName
                        });

                        // Send Notification
                        // تم قبول مهمة العمل كمصلح بالدعوى

                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Reconciliator_AcceptTask).Result.Data?.Value;
                            msg = msg.Replace("{ReconciliatorName}", litigationRequestReformerAssigningRequest.ReformerCitizen.FullName);
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = citizenInfo.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Reformer,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = litigationRequestReformerAssigningRequest.Id,
                                RequestTypeId = RequestType.ReformerTaskrequests,
                                Message = msg
                            });

                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = deptAdminInfoReconciliation.Data.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Employee,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = litigationRequestReformerAssigningRequest.Id,
                                RequestTypeId = RequestType.ReformerTaskrequests,
                                Message = msg
                            });

                            await _notificationService.SendNotifications(
                                 new SendEmailDto()
                                 {
                                     To = new List<string>() { citizenInfo.Email, deptAdminInfoReconciliation.Data.Email },
                                     Subject = "Arbitraor Accept Task",
                                     Body = msg
                                 },
                                 new SMSRequestDto()
                                 {
                                     message_text = msg,
                                     recipients = new List<string>() { citizenInfo.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                                     notification_type = "SMS",

                                 },
                                 applicationNotifications
                                 );
                        }
                        catch (Exception ex)
                        {

                        }


                        break;
                    case LitigationRequestReformerAssigningRequestStatus.Reject:

                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم رفض الدعوى من قبل المصلح " + litigationRequestReformerAssigningRequest.ReformerCitizen.FullName
                        });

                        var department = (await _repository.GetQueryable<SysDepartment, Guid>())?
                                .Where(x => x.DepartmentType == DepartmentType.Reconciliation)
                                .SingleOrDefault();
                        if (department != null)
                        {
                            switch (department.DistributionMode)
                            {
                                case DepartmentDistributionMode.AutoDistribution:
                                    var reformer = (await _repository.GetQueryable<Citizen, Guid>())?
                                        .Include(x => x.ReformerLitigationRequests).Include(x => x.LitigationRequestReformerAssigningRequests)
                                                .Where(x => x.IsReformer == true & x.ReformerStatusId == ReformerStatus.Certified & x.Id != referenceId)
                                                .OrderBy(x => x.ReformerLitigationRequests
                                                .Count(t => t.IsFinished == false)
                                                + x.LitigationRequestReformerAssigningRequests
                                                .Count(x => x.Response == LitigationRequestReformerAssigningRequestStatus.None))
                                                .ThenBy(x => x.ReformerLitigationRequests.Count())
                                                .FirstOrDefault();
                                    if (reformer != null)
                                    {
                                        LitigationRequestReformerAssigningRequest newLitigationRequestReformerAssigningRequest = new LitigationRequestReformerAssigningRequest();
                                        newLitigationRequestReformerAssigningRequest.CitizenId = reformer.Id;
                                        newLitigationRequestReformerAssigningRequest.RequestOn = DateTime.Now;
                                        newLitigationRequestReformerAssigningRequest.LitigationRequestId = litigationRequest.Id;
                                        newLitigationRequestReformerAssigningRequest.Response = LitigationRequestReformerAssigningRequestStatus.None;
                                        await _repository.AddAsync<LitigationRequestReformerAssigningRequest, Guid>(newLitigationRequestReformerAssigningRequest);

                                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                        {
                                            Id = Guid.NewGuid(),
                                            CreatedOn = DateTime.Now,
                                            RequestId = litigationRequest.Id,
                                            ShowForApplicant = false,
                                            LogMessage = "تم إرسال طلب مهمة الصلح للمصلح " + reformer.FullName
                                        });

                                        var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                                        if (!repositorySaveResponse.IsDone)
                                            return repositorySaveResponse;



                                        // Send Notification
                                        // تم إرسال مهمة العمل كمصلح بالدعوى للمصلح التالى

                                        try
                                        {
                                            var reAssignMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_ReAssigned).Result.Data?.Value;
                                            reAssignMsg = reAssignMsg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            reAssignMsg = reAssignMsg.Replace("{ReformerName}", reformer.FullName);

                                            var assignMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Assign).Result.Data?.Value;
                                            assignMsg = assignMsg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            var assignApplicationNotification = new List<SysNotificationDto>();
                                            var reAssignApplicationNotification = new List<SysNotificationDto>();
                                            assignApplicationNotification.Add(new SysNotificationDto()
                                            {
                                                ActorId = reformer.Id,
                                                CreatedOn = DateTime.Now,
                                                ActorTypeId = NotificationActorType.Reformer,
                                                IsViewed = false,
                                                ViewedOn = null,
                                                RequestId = litigationRequestReformerAssigningRequest.Id,
                                                RequestTypeId = RequestType.ReformerTaskrequests,
                                                Message = assignMsg
                                            });

                                            assignApplicationNotification.Add(new SysNotificationDto()
                                            {
                                                ActorId = deptAdminInfoReconciliation.Data.Id,
                                                CreatedOn = DateTime.Now,
                                                ActorTypeId = NotificationActorType.Employee,
                                                IsViewed = false,
                                                ViewedOn = null,
                                                RequestId = litigationRequestReformerAssigningRequest.Id,
                                                RequestTypeId = RequestType.ReformerTaskrequests,
                                                Message = assignMsg
                                            });



                                            reAssignApplicationNotification.Add(new SysNotificationDto()
                                            {
                                                ActorId = deptAdminInfoReconciliation.Data.Id,
                                                CreatedOn = DateTime.Now,
                                                ActorTypeId = NotificationActorType.Employee,
                                                IsViewed = false,
                                                ViewedOn = null,
                                                RequestId = litigationRequestReformerAssigningRequest.Id,
                                                RequestTypeId = RequestType.ReformerTaskrequests,
                                                Message = reAssignMsg
                                            });

                                            var notificationResultReformerAssign = await _notificationService.SendNotifications(
                                                new SendEmailDto()
                                                {
                                                    To = new List<string>() { reformer.Email, deptAdminInfoReconciliation.Data.Email },
                                                    Subject = "Arbitraor Assign Task",
                                                    Body = assignMsg
                                                },
                                                new SMSRequestDto()
                                                {
                                                    message_text = msg,
                                                    recipients = new List<string>() { reformer.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                                                    notification_type = "SMS",

                                                },
                                                assignApplicationNotification
                                                );

                                            await _notificationService.SendNotifications(
                                                new SendEmailDto()
                                                {
                                                    To = new List<string>() { citizenInfo.Email, deptAdminInfoReconciliation.Data.Email },
                                                    Subject = "Arbitraor Accept Task",
                                                    Body = reAssignMsg
                                                },
                                                new SMSRequestDto()
                                                {
                                                    message_text = reAssignMsg,
                                                    recipients = new List<string>() { citizenInfo.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                                                    notification_type = "SMS",

                                                },
                                                reAssignApplicationNotification
                                                );
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                    }

                                    break;
                            }
                        }
                        // Send Notification
                        // تم رفض مهمة العمل كمصلح بالدعوى.
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Reconciliator_RejectTask).Result.Data?.Value;
                            msg = msg.Replace("{ReconciliatorName}", litigationRequestReformerAssigningRequest.ReformerCitizen.FullName);
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = citizenInfo.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Reformer,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = litigationRequestReformerAssigningRequest.Id,
                                RequestTypeId = RequestType.ReformerTaskrequests,
                                Message = msg
                            });

                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = deptAdminInfoReconciliation.Data.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Employee,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = litigationRequestReformerAssigningRequest.Id,
                                RequestTypeId = RequestType.ReformerTaskrequests,
                                Message = msg
                            });

                            await _notificationService.SendNotifications(
                                new SendEmailDto()
                                {
                                    To = new List<string>() { citizenInfo.Email, deptAdminInfoReconciliation.Data.Email },
                                    Subject = "Arbitraor Accept Task",
                                    Body = msg
                                },
                                new SMSRequestDto()
                                {
                                    message_text = msg,
                                    recipients = new List<string>() { citizenInfo.Mobile, deptAdminInfoReconciliation.Data.Mobile },
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
            return await GetLitigationRequestReformerAssigningRequests(litigationRequestReformerAssigningRequest.LitigationRequestId, requestPayload.Id, cancellationToken);
        }
        public async Task<BaseResponse> GetLitigationRequestReformerAssigningRequests(CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.Area)
                         .Include(x => x.City)
                         .Include(x => x.LitigationRequestReformerAssigningRequests)
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
                         .Where(x => x.LitigationRequestReformerAssigningRequests
                         .Where(t => t.CitizenId == referenceId).Count() > 0).OrderByDescending(x => x.CreatedOn).ToList();

            List<CustomLitigationRequestMircoDto> litigationRequestMircoDtos = new List<CustomLitigationRequestMircoDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests)
                {

                    if (litigationRequest.LitigationRequestReformerAssigningRequests.Any())
                    {
                        foreach (var item in litigationRequest.LitigationRequestReformerAssigningRequests.Where(x => x.CitizenId == referenceId))
                        {
                            int reconciliationPeriod = 0;
                            int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Period).Result.Data?.Value, out reconciliationPeriod);



                            int daysPassed = (DateTime.Now - (DateTime)item.RequestOn).Days;
                            item.LitigationRequest.ReconciliationPeriod = Math.Max(0, reconciliationPeriod - daysPassed);


                            CustomLitigationRequestMircoDto litigationRequestMircoDto = new CustomLitigationRequestMircoDto();
                            litigationRequestMircoDto.Id = item.Id;
                            litigationRequestMircoDto.RequestId = litigationRequest.Id;
                            litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                            litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                            litigationRequestMircoDto.ReconciliationPeriod = item.LitigationRequest.ReconciliationPeriod;
                            litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                            litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                            litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                            litigationRequestMircoDto.Response = item.Response;
                            litigationRequestMircoDto.ResponseOn = item.ResponseOn;

                            bool allowAdd = true;
                            if (item.Response == LitigationRequestReformerAssigningRequestStatus.Approve |
                                item.Response == LitigationRequestReformerAssigningRequestStatus.Cancelled |
                                item.Response == LitigationRequestReformerAssigningRequestStatus.Reject)
                            {
                                allowAdd = false;
                            }
                            if (allowAdd)
                                litigationRequestMircoDto.IsDone = "--";
                            else
                                litigationRequestMircoDto.IsDone = "تم";

                            litigationRequestMircoDto.Status = litigationRequest.InternalStatus.RequestStatus;
                            litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                        }
                    }
                }
            }

            return new DataResponse<List<CustomLitigationRequestMircoDto>>()
            {
                Data = litigationRequestMircoDtos,
                IsDone = true
            };

            throw new NotImplementedException();
        }
        public async Task<BaseResponse> GetLitigationRequestReformerAssigningRequestsAll(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequestReformerAssigningRequest> litigationRequestsReformerAssigningRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequestReformerAssigningRequest, Guid>())
                          .Include(x => x.ReformerCitizen)
                         .Include(x => x.LitigationRequest)

                         .Include(x => x.LitigationRequest.ApplicantCitizen)
                         .Include(x => x.LitigationRequest.InternalStatus)
                         .Include(x => x.LitigationRequest.ExternalStatus)

                         .Include(x => x.LitigationRequest.LitigationRequestCategory)
                          .Include(x => x.LitigationRequest.Contract)
                          .Include(x => x.LitigationRequest.Contract.FirstPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.FirstPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.LitigationRequestContractType);


            litigationRequestsReformerAssigningRequests = entitiesQuerable
                .Where(x => x.LitigationRequest.LitigationRequestStage != LitigationRequestStage.Review
                & x.LitigationRequest.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration)
                                       .OrderByDescending(x => x.LitigationRequest.CreatedOn)
                                       .AsSplitQuery()
                                       .AsNoTracking()
                                       .GetPaged(pageIndex, pageSize);


            List<CustomLitigationRequestMircoAllDto> litigationRequestMircoDtos = new List<CustomLitigationRequestMircoAllDto>();
            if (litigationRequestsReformerAssigningRequests != null)
            {

                int reconciliationPeriod = 0;
                int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Period).Result.Data?.Value, out reconciliationPeriod);


                foreach (var litigationRequestReformerAssigningRequest in litigationRequestsReformerAssigningRequests.Results)
                {
                    int daysPassed = (DateTime.Now - (DateTime)litigationRequestReformerAssigningRequest.RequestOn).Days;
                    litigationRequestReformerAssigningRequest.LitigationRequest.ReconciliationPeriod = Math.Max(0, reconciliationPeriod - daysPassed);
                }
                foreach (var litigationRequest in litigationRequestsReformerAssigningRequests.Results)
                {
                    CustomLitigationRequestMircoAllDto litigationRequestMircoDto = new CustomLitigationRequestMircoAllDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestId = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.LitigationRequest.CreatedOn;
                    litigationRequestMircoDto.ReconciliationPeriod = litigationRequest.LitigationRequest.ReconciliationPeriod;
                    litigationRequestMircoDto.RequestCode = litigationRequest.LitigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.LitigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.LitigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.LitigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.LitigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.LitigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.LitigationRequest.Contract.SecondPartyCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.LitigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.Response = litigationRequest.Response;
                    litigationRequestMircoDto.ReformerName = litigationRequest.ReformerCitizen?.FullName;
                    litigationRequestMircoDto.ResponseOn = litigationRequest.ResponseOn;

                    bool allowAdd = true;


                    if (litigationRequest.Response == LitigationRequestReformerAssigningRequestStatus.Approve |
                        litigationRequest.Response == LitigationRequestReformerAssigningRequestStatus.Cancelled |
                        litigationRequest.Response == LitigationRequestReformerAssigningRequestStatus.Reject)
                    {
                        allowAdd = false;
                    }


                    if (allowAdd)
                        litigationRequestMircoDto.IsDone = "--";
                    else
                        litigationRequestMircoDto.IsDone = "تم";

                    litigationRequestMircoDto.Status = litigationRequest.LitigationRequest.InternalStatus.RequestStatus;
                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                }
            }
            return new DataResponse<PagedResult<CustomLitigationRequestMircoAllDto>>
            {
                Data =
                              new PagedResult<CustomLitigationRequestMircoAllDto>
                              {
                                  Results = litigationRequestMircoDtos,
                                  CurrentPage = litigationRequestsReformerAssigningRequests.CurrentPage,
                                  PageCount = litigationRequestsReformerAssigningRequests.PageCount,
                                  PageSize = litigationRequestsReformerAssigningRequests.PageSize,
                                  TotalCount = litigationRequestsReformerAssigningRequests.TotalCount
                              },
                IsDone = true
            };
        }

        public async Task<BaseResponse> GetLitigationRequestReformerAssigningRequests(Guid requestId, Guid LitigationRequestReformerAssigningRequestId, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.Area)
                         .Include(x => x.City)
                         .Include(x => x.LitigationRequestReformerAssigningRequests)
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
                         .Where(x => x.LitigationRequestReformerAssigningRequests
                         .Where(t => t.Id == LitigationRequestReformerAssigningRequestId).Count() > 0).AsNoTracking().ToList().FirstOrDefault();

            if (litigationRequest != null)
            {
                CustomLitigationRequestMircoDto litigationRequestMircoDto = new CustomLitigationRequestMircoDto();

                litigationRequestMircoDto.Id = litigationRequest.Id;
                litigationRequestMircoDto.RequestId = requestId;
                litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                litigationRequestMircoDto.Response = litigationRequest.LitigationRequestReformerAssigningRequests.FirstOrDefault()?.Response;
                litigationRequestMircoDto.ResponseOn = litigationRequest.LitigationRequestReformerAssigningRequests.FirstOrDefault()?.ResponseOn;

                bool allowAdd = true;

                if (litigationRequest.LitigationRequestReformerAssigningRequests.Any())
                {
                    if (litigationRequest.LitigationRequestReformerAssigningRequests.FirstOrDefault()?.Response == LitigationRequestReformerAssigningRequestStatus.Approve |
                        litigationRequest.LitigationRequestReformerAssigningRequests.FirstOrDefault()?.Response == LitigationRequestReformerAssigningRequestStatus.Cancelled |
                        litigationRequest.LitigationRequestReformerAssigningRequests.FirstOrDefault()?.Response == LitigationRequestReformerAssigningRequestStatus.Reject)
                    {
                        allowAdd = false;
                    }
                }

                if (allowAdd)
                    litigationRequestMircoDto.IsDone = "--";
                else
                    litigationRequestMircoDto.IsDone = "تم";

                litigationRequestMircoDto.Status = litigationRequest.InternalStatus.RequestStatus;

                return new DataResponse<CustomLitigationRequestMircoDto>()
                {
                    Data = litigationRequestMircoDto,
                    IsDone = true
                };

            }
            throw new NotImplementedException();
        }
    }
}