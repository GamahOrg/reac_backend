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

            var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                .Where(c => c.Id == referenceId)
                .FirstOrDefault();

            if (citizenInfo?.SigntureFileId == null)
            {
                return new BaseResponse() { IsDone = false, Message = "يرجى تسجيل التوقيع الالكتروني الخاص بك" };
            }

            var litigationRequestReformerAssigningRequest = (await _repository.GetQueryable<LitigationRequestReformerAssigningRequest, Guid>())
                            .Include(l => l.ReformerCitizen)
                         .Where(x => x.Id == requestPayload.Id).ToList().FirstOrDefault();

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestExtendPeriodRequests)
                        .Include(x => x.Contract.FirstPartyCitizen)
                        .Include(x => x.Contract.SecondPartyCitizen)
                        .Include(x => x.Contract.FirstPartyFacility.Citizen)
                        .Include(x => x.Contract.SecondPartyFacility.Citizen)
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
            
                var msg = "";
                string body;
                var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
                var recipients = new HashSet<string>
                       {
                            citizenInfo.Mobile,
                            litigationRequest.Contract.FirstPartyCitizen?.Mobile,
                            litigationRequest.Contract.FirstPartyFacility?.Citizen.Mobile,
                            litigationRequest.Contract.SecondPartyCitizen?.Mobile,
                            litigationRequest.Contract.SecondPartyFacility?.Citizen.Mobile,
                            deptAdminInfoReconciliation.Data.Mobile
                        }.Where(m => !string.IsNullOrEmpty(m))
                         .ToList();


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
                            var settingResponse = await _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Reconciliator_AcceptTask);

                            msg = settingResponse.Data?.Value;
                            msg = msg.Replace("{ReconciliatorName}", litigationRequestReformerAssigningRequest.ReformerCitizen.FullName);
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", msg);
                                body = body.Replace("{ReconciliatorName}", litigationRequestReformerAssigningRequest.ReformerCitizen.FullName);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
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
                            }); applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = litigationRequest.Contract.FirstPartyCitizen is not null ? litigationRequest.Contract.FirstPartyCitizen.Id : litigationRequest.Contract.FirstPartyFacility.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Plaintiff,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = litigationRequestReformerAssigningRequest.Id,
                                RequestTypeId = RequestType.ReformerTaskrequests,
                                Message = msg
                            }); applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = litigationRequest.Contract.SecondPartyCitizen is not null ? litigationRequest.Contract.SecondPartyCitizen.Id : litigationRequest.Contract.SecondPartyCitizen.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Defendant,
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
                                    To = new List<string>() { citizenInfo.Email,litigationRequest.Contract.FirstPartyCitizen is not null ? litigationRequest.Contract.FirstPartyCitizen.Email : litigationRequest.Contract.FirstPartyFacility.Citizen.Email,
                                         litigationRequest.Contract.SecondPartyCitizen is not null ?  litigationRequest.Contract.SecondPartyCitizen.Email : litigationRequest.Contract.SecondPartyFacility.Citizen.Email, deptAdminInfoReconciliation.Data.Email },
                                    Subject = "قبول المصلح المهمة",
                                    Body = body
                                },
                                new SMSRequestDto()
                                {
                                    message_text = msg,
                                    recipients = recipients,
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
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string reAssignBody;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                reAssignBody = await streamReader.ReadToEndAsync();
                                                reAssignBody = reAssignBody?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_ReAssigned).Result.Data?.Value);
                                                reAssignBody = reAssignBody?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                reAssignBody = reAssignBody.Replace("{ReformerName}", reformer.FullName);
                                            }

                                            var assignMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Assign).Result.Data?.Value;
                                            assignMsg = assignMsg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string assignBody;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                assignBody = await streamReader.ReadToEndAsync();
                                                assignBody = assignBody?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Assign).Result.Data?.Value);
                                                assignBody = assignBody?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
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
                                                    Body = assignBody
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
                                                   Body = reAssignBody
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
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Reconciliator_RejectTask).Result.Data?.Value);
                                body = body.Replace("{ReconciliatorName}", litigationRequestReformerAssigningRequest.ReformerCitizen.FullName);
                                body = body.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            }
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
                                   Body = body
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
        public async Task<BaseResponse> GetLitigationRequestReformerAssigningRequests(int pageIndex, int pageSize, CancellationToken cancellationToken, bool ascending = false)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            // Check if reformer is certified
            if(referenceId != Guid.Empty)
            {
                var reformerStatusId = (await _repository.GetQueryable<Citizen, Guid>())
                    .FirstOrDefault(x => x.Id == referenceId)
                    .ReformerStatusId;

                if (reformerStatusId == ReformerStatus.Stopped)
                {
                    return new BaseResponse() { IsDone = false, Message = "تم ايقاف الحساب الخاص بك" };
                }
            }
            

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
                         .Where(t => t.CitizenId == referenceId).Count() > 0).OrderByDescending(x => x.SentOn).AsNoTracking().GetPaged(pageIndex, pageSize);


            litigationRequests.Results = (ascending
                ? litigationRequests.Results.OrderBy(x => x.SentOn).ToList()
                : litigationRequests.Results.OrderByDescending(x => x.SentOn).ToList());


            List<CustomLitigationRequestMircoDto> litigationRequestMircoDtos = new List<CustomLitigationRequestMircoDto>();
            if (litigationRequests.Results != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    if (litigationRequest.LitigationRequestReformerAssigningRequests.Any())
                    {
                        foreach (var item in litigationRequest.LitigationRequestReformerAssigningRequests.Where(x => x.CitizenId == referenceId))
                        {
                            int reconciliationPeriod = 0;
                            int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Period).Result.Data?.Value, out reconciliationPeriod);

                            int daysPassed = (DateTime.Now - (DateTime)item.RequestOn).Days;
                            item.LitigationRequest.ReconciliationPeriod = Math.Max(0, reconciliationPeriod - daysPassed);

                            CustomLitigationRequestMircoDto litigationRequestMircoDto = new CustomLitigationRequestMircoDto
                            {
                                Id = item.Id,
                                RequestId = litigationRequest.Id,
                                RequestDate = litigationRequest.CreatedOn,
                                RequestCode = litigationRequest.RequestCode,
                                ReconciliationPeriod = item.LitigationRequest.ReconciliationPeriod,
                                ReformerAssigningDate = item.RequestOn,
                                FirstPartyName = litigationRequest.Contract.FirstPartyCitizen == null
                                    ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName
                                    : litigationRequest.Contract.FirstPartyCitizen.FullName,
                                SecondPartyName = litigationRequest.Contract.SecondPartyCitizen == null
                                    ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName
                                    : litigationRequest.Contract.SecondPartyCitizen.FullName,
                                RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName,
                                Response = item.Response,
                                ResponseOn = item.ResponseOn,
                                IsDone = (item.Response == LitigationRequestReformerAssigningRequestStatus.Approve ||
                                           item.Response == LitigationRequestReformerAssigningRequestStatus.Cancelled ||
                                           item.Response == LitigationRequestReformerAssigningRequestStatus.Reject) ? "تم" : "--",
                                Status = litigationRequest.InternalStatus.RequestStatus
                            };

                            litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                        }
                    }
                }
            }

            return new DataResponse<PagedResult<CustomLitigationRequestMircoDto>>
            {
                Data =
                              new PagedResult<CustomLitigationRequestMircoDto>
                              {
                                  Results = litigationRequestMircoDtos,
                                  CurrentPage = litigationRequests.CurrentPage,
                                  PageCount = litigationRequests.PageCount,
                                  PageSize = litigationRequests.PageSize,
                                  TotalCount = litigationRequests.TotalCount
                              },
                IsDone = true
            };
        }
        public async Task<BaseResponse> GetLitigationRequestReformerAssigningRequestsAll(int pageIndex, int pageSize, CancellationToken cancellationToken, bool ascending = false)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var entitiesQueryable = (await _repository.GetQueryable<LitigationRequestReformerAssigningRequest, Guid>())
                .Include(x => x.ReformerCitizen)
                .Include(x => x.LitigationRequest)
                .Include(x => x.LitigationRequest.ApplicantCitizen)
                .Include(x => x.LitigationRequest.DefendantCitizen.Nationality)
                .Include(x => x.LitigationRequest.DefendantFacility)
                .Include(x => x.LitigationRequest.InternalStatus)
                .Include(x => x.LitigationRequest.ExternalStatus)
                .Include(x => x.LitigationRequest.LitigationRequestCategory)
                .Include(x => x.LitigationRequest.Contract)
                .Include(x => x.LitigationRequest.Contract.FirstPartyFacility)
                .Include(x => x.LitigationRequest.Contract.FirstPartyCitizen)
                .Include(x => x.LitigationRequest.Contract.SecondPartyFacility)
                .Include(x => x.LitigationRequest.Contract.SecondPartyCitizen)
                //.Include(x => x.LitigationRequest.LitigationRequestReformerAssigningRequests)
                .Include(x => x.LitigationRequest.Contract.LitigationRequestContractType);


            var litigationRequestsReformerAssigningRequestsQuery = entitiesQueryable
                .Where(x => x.LitigationRequest.LitigationRequestStage != LitigationRequestStage.Review
                             && x.LitigationRequest.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration);

            litigationRequestsReformerAssigningRequestsQuery = ascending
                ? litigationRequestsReformerAssigningRequestsQuery.OrderBy(x => x.LitigationRequest.SentOn)
                : litigationRequestsReformerAssigningRequestsQuery.OrderByDescending(x => x.LitigationRequest.SentOn);


            var totalCount = await litigationRequestsReformerAssigningRequestsQuery.CountAsync(cancellationToken);


            var results = await litigationRequestsReformerAssigningRequestsQuery
                .AsSplitQuery()
                .AsNoTracking()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);


            List<CustomLitigationRequestMircoAllDto> litigationRequestMircoDtos = new List<CustomLitigationRequestMircoAllDto>();
            if (results != null)
            {
                int reconciliationPeriod = 0;
                int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Period).Result.Data?.Value, out reconciliationPeriod);

                foreach (var litigationRequestReformerAssigningRequest in results)
                {
                    int daysPassed = (DateTime.Now - (DateTime)litigationRequestReformerAssigningRequest.RequestOn).Days;
                    litigationRequestReformerAssigningRequest.LitigationRequest.ReconciliationPeriod = Math.Max(0, reconciliationPeriod - daysPassed);
                }

                foreach (var litigationRequest in results)
                {
                    //var RejectReason1 = (await _repository.GetQueryable<LitigationRequestReformerAssigningRequest, Guid>()).Where(s => s.LitigationRequestId == litigationRequest.LitigationRequestId && s.Response == LitigationRequestReformerAssigningRequestStatus.Cancelled)?.OrderBy(s => s.ResponseOn);
                    var RejectReason = (await _repository.GetQueryable<LitigationRequestReformerAssigningRequest, Guid>()).Where(s => s.LitigationRequestId == litigationRequest.LitigationRequestId && s.Response == LitigationRequestReformerAssigningRequestStatus.Cancelled)?.OrderBy(s=>s.ResponseOn).LastOrDefault(s=>s.Reason != null)?.Reason;
                    CustomLitigationRequestMircoAllDto litigationRequestMircoDto = new CustomLitigationRequestMircoAllDto
                    {
                        Id = litigationRequest.Id,
                        RequestId = litigationRequest.LitigationRequest.Id,
                        RequestDate = litigationRequest.LitigationRequest.CreatedOn,
                        ReformerAssigningDate = litigationRequest.RequestOn,
                        ReconciliationPeriod = litigationRequest.LitigationRequest.ReconciliationPeriod,
                        RequestCode = litigationRequest.LitigationRequest.RequestCode,
                        FirstPartyName = litigationRequest.LitigationRequest.Contract.FirstPartyCitizen == null
                            ? litigationRequest.LitigationRequest.Contract.FirstPartyFacility.ArabicFacilityName
                            : litigationRequest.LitigationRequest.Contract.FirstPartyCitizen.FullName,
                        SecondPartyName = litigationRequest.LitigationRequest.DefendantCitizenId == null
                            ? litigationRequest.LitigationRequest.DefendantFacility.ArabicFacilityName
                            : litigationRequest.LitigationRequest.DefendantCitizen.FullName,
                        RequestTypeName = litigationRequest.LitigationRequest.Contract.LitigationRequestContractType.TypeName,
                        Response = litigationRequest.Response,
                        RejectReason = RejectReason,
                        ReformerName = litigationRequest.ReformerCitizen?.FullName,
                        ResponseOn = litigationRequest.ResponseOn,
                        IsDone = (litigationRequest.Response == LitigationRequestReformerAssigningRequestStatus.Approve ||
                                   litigationRequest.Response == LitigationRequestReformerAssigningRequestStatus.Cancelled ||
                                   litigationRequest.Response == LitigationRequestReformerAssigningRequestStatus.Reject) ? "تم" : "--",
                        Status = litigationRequest.LitigationRequest.InternalStatus.RequestStatus
                    };

                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                }
            }

            return new DataResponse<PagedResult<CustomLitigationRequestMircoAllDto>>
            {
                Data = new PagedResult<CustomLitigationRequestMircoAllDto>
                {
                    Results = litigationRequestMircoDtos,
                    CurrentPage = pageIndex,
                    PageCount = (int)Math.Ceiling((double)totalCount / pageSize),
                    PageSize = pageSize,
                    TotalCount = totalCount
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