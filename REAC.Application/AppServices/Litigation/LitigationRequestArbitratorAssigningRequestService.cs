using AutoMapper;
using Core.Contracts;
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

            var litigationRequestArbitratorAssigningRequest = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())
                         .Include(l => l.ArbitratorCitizen)
                         .Where(x => x.Id == requestPayload.Id).ToList().FirstOrDefault();

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestArbitratorNominationLists)
                        .ThenInclude(x => x.ArbitratorCitizen)
                           .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                        .ThenInclude(x => x.ArbitratorChairmanCitizen)
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
                var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                    .Where(c => c.Id == referenceId)
                    .FirstOrDefault();
                var msg = "";
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
                                break;
                        }

                        // Send Notification
                        // تم قبول طلب المهمة من المحكم
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_AcceptTask).Result.Data?.Value;
                            msg = msg.Replace("{ArbitratorName}", litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName);
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
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
                                   Body = msg
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
                                var arbitratorsList = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsSent != true).OrderBy(x => x.TotalPriority).ThenBy(x => x.DefendantPriority).ToList();
                                if (arbitratorsList != null)
                                {
                                    if (arbitratorsList.Count > 0)
                                    {

                                        LitigationRequestArbitratorAssigningRequest nextLitigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                                        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                                        nextLitigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                                        nextLitigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.Arbitrator;
                                        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                                        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = arbitratorsList.FirstOrDefault().Id;
                                        nextLitigationRequestArbitratorAssigningRequest.CitizenId = arbitratorsList.FirstOrDefault().ArbitratorCitizenId;
                                        nextLitigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                                        await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(nextLitigationRequestArbitratorAssigningRequest);
                                        arbitratorsList.FirstOrDefault().IsSent = true;

                                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                        {
                                            Id = Guid.NewGuid(),
                                            CreatedOn = DateTime.Now,
                                            RequestId = litigationRequest.Id,
                                            ShowForApplicant = false,
                                            LogMessage = "تم إرسال طلب مهمة تحكيم للمحكم " + arbitratorsList.FirstOrDefault()?.ArbitratorCitizen.FullName + " كمحكم"
                                        });
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
                                    LogMessage = "تم رفض المهمة من المحكم " + litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName + " كرئيس هيئة التحكيم"
                                });
                                var chairmanArbitratorsList = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Where(x => x.IsSent != true).OrderBy(x => x.TotalPriority).ToList();
                                if (chairmanArbitratorsList != null)
                                {
                                    if (chairmanArbitratorsList.Count > 0)
                                    {
                                        LitigationRequestArbitratorAssigningRequest nextLitigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                                        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                                        nextLitigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                                        nextLitigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.ArbitratorsChairman;
                                        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                                        //nextLitigationRequestArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = chairmanArbitratorsList.FirstOrDefault().Id;
                                        nextLitigationRequestArbitratorAssigningRequest.CitizenId = chairmanArbitratorsList.FirstOrDefault().ArbitratorChairmanCitizenId;
                                        nextLitigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                                        await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(nextLitigationRequestArbitratorAssigningRequest);
                                        chairmanArbitratorsList.FirstOrDefault().IsSent = true;

                                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                        {
                                            Id = Guid.NewGuid(),
                                            CreatedOn = DateTime.Now,
                                            RequestId = litigationRequest.Id,
                                            ShowForApplicant = false,
                                            LogMessage = "تم إرسال طلب مهمة للمحكم " + chairmanArbitratorsList.FirstOrDefault()?.ArbitratorChairmanCitizen.FullName + " كرئيس هيئة التحكيم"
                                        });
                                    }
                                }
                                break;
                        }
                        // Send Notification
                        // تم رفض طلب المهمة من المحكم

                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_RejectTask).Result.Data?.Value;
                            msg = msg.Replace("{ArbitratorName}", litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName);
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
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
                                   Body = msg
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

            litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestArbitratorLists)
                        .Where(x => x.Id == litigationRequestArbitratorAssigningRequest.LitigationRequestId)
                        .SingleOrDefault();

            switch (litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId)
            {
                case ArbitratorAssigningRequestType.Arbitrator:
                    if (litigationRequest.LitigationRequestArbitratorLists.Count == litigationRequest.ArbitratorsCount - 1)
                        litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAddArbitratorChairmanListNomination;
                    break;
            }

            saveResponse = await _repository.SaveChangesAsync(cancellationToken);
            if (!saveResponse.IsDone)
                return saveResponse;


            return await GetLitigationRequestArbitratorAssigningRequests(litigationRequestArbitratorAssigningRequest.LitigationRequestId, requestPayload.Id, cancellationToken);
        }
        public async Task<BaseResponse> GetLitigationRequestArbitratorAssigningRequests(CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

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
                         .Where(t => t.CitizenId == referenceId).Count() > 0).OrderByDescending(x => x.CreatedOn).AsNoTracking().ToList();

            List<CustomLitigationRequestMircoArbitratorDto> litigationRequestMircoDtos = new List<CustomLitigationRequestMircoArbitratorDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests)
                {
                    foreach (var litigationRequestArbitratorAssigningRequest in litigationRequest.LitigationRequestArbitratorAssigningRequests
                        .Where(x => x.CitizenId == referenceId).OrderByDescending(x => x.RequestOn))
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

            return new DataResponse<List<CustomLitigationRequestMircoArbitratorDto>>()
            {
                Data = litigationRequestMircoDtos,
                IsDone = true
            };

            throw new NotImplementedException();
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
                                       .OrderByDescending(x => x.LitigationRequest.CreatedOn)
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
                    litigationRequestMircoDto.RequestDate = litigationRequest.LitigationRequest.CreatedOn;
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
    }
}