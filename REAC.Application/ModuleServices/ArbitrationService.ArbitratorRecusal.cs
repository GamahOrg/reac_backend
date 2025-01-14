using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.ModuleServices
{


    public partial class ArbitrationService
    {
        #region RecusalRequest
        public async Task<BaseResponse> AddArbitrationArbitratorRecusalRequest(LitigationRequestArbitrationArbitratorRecusalRequestCreateDto requestPayload)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                                      .Include(x => x.LitigationRequestArbitratorLists)
                                      .ThenInclude(x => x.ArbitratorCitizen)
                                      .Include(x => x.LitigationRequestArbitratorRecusalRequests)
                                      .ThenInclude(x => x.LitigationRequestArbitratorRecusalRequestFiles)
                                      .Include(x => x.LitigationRequestArbitrationReports)
                                      .Include(x => x.Contract)
                                      .Include(x => x.Contract.LitigationRequestContractType)
                                      .Include(x => x.Contract.ContractFile)
                                      .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                                      .Include(x => x.Contract.FoundingLetterFile)
                                      .Include(x => x.Contract.InternalRegulationsFile)
                                      .Include(x => x.Contract.FirstPartyFacility)
                                      .Include(x => x.Contract.FirstPartyCitizen)
                                      .Include(x => x.Contract.SecondPartyFacility)
                                      .Include(x => x.Contract.SecondPartyCitizen).Include(x => x.Contract.LitigationRequestContractType)
                                      .Where(x => x.Id == requestPayload.LitigationRequestId).SingleOrDefault();

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                bool allowAdd = true;

                LitigationRequestParty litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.None;

                if (!litigationRequest.ArbitratorsNominationListSelectionDoneDateTime.HasValue)
                    allowAdd = false;
                else
                {
                    if (litigationRequest.ApplicantCitizenId == referenceId)
                    {
                        litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                    }
                    if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                        }
                    }
                    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                        }
                    }
                    if (litigationRequest.LitigationRequestArbitrationReports.FirstOrDefault()?.LitigationRequestArbitrationReportExplanationRequests != null)
                    {
                        if (litigationRequest.LitigationRequestArbitrationReports.FirstOrDefault()?.LitigationRequestArbitrationReportExplanationRequests.Count(x => x.RequestArbitrationReportExplanationRequestParty == litigationRequestArbitrationReportExplanationRequestParty) > 0)
                            allowAdd = false;

                    }
                }

                if (allowAdd)
                {
                    if (litigationRequestArbitrationReportExplanationRequestParty == LitigationRequestParty.None)
                    {
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "غير متاح"
                        };
                    }

                    int LitigationRequestArbitratorRecusalRequestPeriod = 0;
                    int.TryParse(_settingService.GetBySettingKey(SysSettingKeyConstants.SLA_Arbitration_LitigationRequestArbitratorRecusalRequestPeriod).Result.Data?.Value, out LitigationRequestArbitratorRecusalRequestPeriod);
                    if (LitigationRequestArbitratorRecusalRequestPeriod > 0)
                    {
                        if (litigationRequest.ArbitratorsNominationListSelectionDoneDateTime.HasValue)
                        {
                            if ((DateTime.Now - litigationRequest.ArbitratorsNominationListSelectionDoneDateTime.Value).TotalHours >= LitigationRequestArbitratorRecusalRequestPeriod)
                            {
                                return new BaseResponse() { IsDone = false, Message = "لا يمكن إضافة طلب رد المحكم لتجاوز الفترة المتاحة لإضافة الطلب" };
                            }
                        }
                    }

                    LitigationRequestArbitratorRecusalRequest litigationRequestArbitratorRecusalRequest = new LitigationRequestArbitratorRecusalRequest()
                    {
                        Id = Guid.NewGuid(),
                        RequestOn = DateTime.Now,
                        Reason = requestPayload.Reason,
                        RequestParty = litigationRequestArbitrationReportExplanationRequestParty,
                        ArbitratorCitizenId = requestPayload.ArbitratorCitizenId,
                        LitigationRequestId = requestPayload.LitigationRequestId
                    };

                    var citizen = (await _repository.GetQueryable<Citizen, Guid>()).Where(x => x.Id == requestPayload.ArbitratorCitizenId).FirstOrDefault();

                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationRequest.Id,
                        ShowForApplicant = false,
                        LogMessage = "تم إرسال طلب رد المحكم " + citizen.FullName + " من قبل " + ((litigationRequestArbitrationReportExplanationRequestParty == LitigationRequestParty.Plaintiff) ? "المدعى" : "المدعى عليه")
                    });

                    var recusalRequest = await _repository.AddAsync<LitigationRequestArbitratorRecusalRequest, Guid>(litigationRequestArbitratorRecusalRequest);

                    var saveResponse = await _repository.SaveChangesAsync();
                    if (!saveResponse.IsDone)
                        return saveResponse;


                    // Send Notification
                    // تم أضافة طلب رد محكم
                    try
                    {
                        var applicantInfo = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(c => c.Id == referenceId);
                        var arbitratorInfo = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(c => c.Id == litigationRequestArbitratorRecusalRequest.ArbitratorCitizenId);
                        var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);


                        var msg = "";
                        msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitraorRecusal_AddRequest).Result.Data?.Value;
                        msg = msg?.Replace("{ArbitratorName}", arbitratorInfo?.FullName);
                        msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);

                        List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                        new SysNotificationDto{
                        ActorId = applicantInfo.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Plaintiff,
                        Message=msg
                        },
                           new SysNotificationDto{
                        ActorId = (Guid)arbitrationAdmin.Data?.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Employee,
                        Message=msg
                        }

                        };
                        _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                        {
                            To = new List<string>() { applicantInfo.Email, arbitrationAdmin.Data?.Email },
                            Subject = "أضافة طلب رد محكم",
                            Body = msg
                        }, null, notifications);

                    }
                    catch (Exception e)
                    {


                    }


                    requestPayload.LitigationRequestArbitrationArbitratorRecusalRequestFiles.ForEach(t =>
                    {
                        _repository.AddAsync<LitigationRequestArbitratorRecusalRequestFile, Guid>(new LitigationRequestArbitratorRecusalRequestFile()
                        {
                            Id = Guid.NewGuid(),
                            RecusalRequestFileId = t,
                            LitigationRequestArbitratorRecusalRequestId = litigationRequestArbitratorRecusalRequest.Id
                        });
                    });

                    var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;

                }
                else
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لا يمكن إضافة طلب جديد"
                    };

                }
            }
            catch (Exception ex)
            {
                new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
            return await GetLitigationRequestForArbitratorRecusalRequest(requestPayload.LitigationRequestId);
        }
        public async Task<BaseResponse> GetLitigationRequestForArbitratorRecusalRequest(Guid requestId)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                     .Include(x => x.LitigationRequestArbitratorRecusalRequests)
                         .ThenInclude(x => x.ArbitratorCitizen)
                     .Include(x => x.LitigationRequestArbitratorRecusalRequests)
                         .ThenInclude(x => x.LitigationRequestArbitratorRecusalRequestFiles)
                         .ThenInclude(x => x.RecusalRequestFile)
                         .Include(x => x.ApplicantCitizen)
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
                         .Where(x => x.Id == requestId).AsNoTracking().ToList().FirstOrDefault();

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };

            bool isDone = false;
            bool allowAdd = true;

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            LitigationRequestParty litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.None;
            if (litigationRequest.ApplicantCitizenId == referenceId)
            {
                litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
            }
            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
            {
                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                {
                    litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                }
            }
            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
            {
                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                {
                    litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                }
            }
            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
            {
                if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                {
                    litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                }
            }
            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
            {
                if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                {
                    litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                }
            }

            if (litigationRequest.IsFinished.HasValue)
            {
                if (litigationRequest.IsFinished.Value)
                {
                    allowAdd = false;
                    isDone = true;
                }
            }

            var requestArbitrationReportExplanationRequest = litigationRequest?.LitigationRequestArbitratorRecusalRequests
                            .Where(x => x.RequestParty == litigationRequestArbitrationReportExplanationRequestParty).FirstOrDefault();

            if (!isDone)
            {
                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                    allowAdd = false;

                if (requestArbitrationReportExplanationRequest is not null)
                    allowAdd = false;
            }
            var reportExplanationRequest = _mapper.Map<LitigationRequestArbitratorRecusalRequestGetDto>(requestArbitrationReportExplanationRequest);



            return new DataResponse<LitigationRequestForArbitratorRecusalRequestDto>()
            {
                Data = new LitigationRequestForArbitratorRecusalRequestDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    request = reportExplanationRequest
                },
                IsDone = true,
            };

        }
        public async Task<BaseResponse> GetLitigationRequestForArbitratorRecusalRequest(int pageIndex, int pageSize)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;
            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                                   .Include(x => x.LitigationRequestArbitratorRecusalRequests)
                                      .ThenInclude(x => x.LitigationRequestArbitratorRecusalRequestFiles)
                                     .Include(x => x.LitigationRequestArbitrationReports)
                                     .ThenInclude(x => x.LitigationRequestArbitrationReportExplanationRequests)
                                     .Include(x => x.ApplicantCitizen)
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
                                     .Include(x => x.ExternalStatus);

            litigationRequests = entitiesQuerable
                .Where(x => x.RequestCode != null

                                        & x.ApplicantCitizenId == referenceId & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                        & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation

                                      | x.RequestCode != null & x.ArbitratorSelectionDuration.HasValue
                                        & x.Contract.FirstPartyCitizenId == referenceId
                                        & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                        & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation
                                      | x.RequestCode != null & x.ArbitratorSelectionDuration.HasValue
                                        & x.Contract.SecondPartyCitizenId == referenceId
                                        & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                        & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation

                                      | x.RequestCode != null & x.ArbitratorSelectionDuration.HasValue
                                        & x.Contract.FirstPartyFacility.CitizenId == referenceId
                                        & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                        & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation

                                      | x.RequestCode != null & x.ArbitratorSelectionDuration.HasValue
                                        & x.Contract.SecondPartyFacility.CitizenId == referenceId
                                        & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                        & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation

                                      ).OrderByDescending(x => x.CreatedOn)
                                        .AsSplitQuery()
                                        .AsNoTracking()
                                        .GetPaged(pageIndex, pageSize);

            List<LitigationRequestMircoDto> litigationRequestMircoDtos = new List<LitigationRequestMircoDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    LitigationRequestMircoDto litigationRequestMircoDto = new LitigationRequestMircoDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                    bool allowAdd = true;
                    bool isDone = false;

                    LitigationRequestParty litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.None;
                    if (litigationRequest.ApplicantCitizenId == referenceId)
                    {
                        litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                    }
                    if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                        }
                    }
                    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                        }
                    }

                    if (litigationRequest.IsFinished.HasValue)
                    {
                        if (litigationRequest.IsFinished.Value)
                        {
                            allowAdd = false;
                            isDone = true;
                        }
                    }
                    var requestArbitrationReportExplanationRequest = litigationRequest?.LitigationRequestArbitratorRecusalRequests
                         .Where(x => x.RequestParty == litigationRequestArbitrationReportExplanationRequestParty).FirstOrDefault();
                    if (requestArbitrationReportExplanationRequest is not null)
                    {
                        if (requestArbitrationReportExplanationRequest.ApproveRejectStatus.HasValue)
                            litigationRequestMircoDto.Response = requestArbitrationReportExplanationRequest.ApproveRejectStatus.Value;
                        else
                            litigationRequestMircoDto.Response = ApproveRejectStatus.NoResponse;

                    }

                    if (!isDone)
                    {
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                            allowAdd = false;
                        else
                        {
                            if (litigationRequest?.ArbitratorCitizenId != null)
                                allowAdd = false;

                            if (litigationRequest?.LitigationRequestArbitratorRecusalRequests.Count > 0)
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

            return new DataResponse<PagedResult<LitigationRequestMircoDto>>
            {
                Data =
                  new PagedResult<LitigationRequestMircoDto>
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


        // BackOffice
        public async Task<BaseResponse> GetLitigationRequestForArbitratorRecusalRequestForArbitrator(int pageIndex, int pageSize)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequestArbitratorRecusalRequest> litigationRequestArbitratorRecusalRequests = null;
            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequestArbitratorRecusalRequest, Guid>())
                                                      .Include(x => x.LitigationRequest.Area)
                                      .Include(x => x.LitigationRequestArbitratorRecusalRequestFiles)
                         .Include(x => x.LitigationRequest.ApplicantCitizen)
                         .Include(x => x.LitigationRequest.Contract)
                         .Include(x => x.LitigationRequest.Contract.ContractFile)
                         .Include(x => x.LitigationRequest.Contract.AssociationRegistrationCertificateFile)
                         .Include(x => x.LitigationRequest.Contract.FoundingLetterFile)
                         .Include(x => x.LitigationRequest.Contract.InternalRegulationsFile)
                         .Include(x => x.LitigationRequest.Contract.FirstPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.FirstPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.LitigationRequestContractType)
                         .Include(x => x.LitigationRequest.InternalStatus);

            litigationRequestArbitratorRecusalRequests = entitiesQuerable
                                        .OrderByDescending(x => x.LitigationRequest.CreatedOn)
                                        .AsSplitQuery()
                                        .AsNoTracking()
                                        .GetPaged(pageIndex, pageSize);

            List<LitigationRequestMircoExplanationRequestDto> litigationRequestMircoDtos = new List<LitigationRequestMircoExplanationRequestDto>();
            if (litigationRequestArbitratorRecusalRequests != null)
            {
                foreach (var litigationRequestArbitratorRecusalRequest in litigationRequestArbitratorRecusalRequests.Results)
                {

                    LitigationRequestMircoExplanationRequestDto litigationRequestMircoDto = new LitigationRequestMircoExplanationRequestDto();
                    litigationRequestMircoDto.Id = litigationRequestArbitratorRecusalRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequestArbitratorRecusalRequest.LitigationRequest.CreatedOn;

                    litigationRequestMircoDto.RequestCode = litigationRequestArbitratorRecusalRequest.LitigationRequest.RequestCode;

                    litigationRequestMircoDto.FirstPartyName = (litigationRequestArbitratorRecusalRequest.LitigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequestArbitratorRecusalRequest.LitigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequestArbitratorRecusalRequest.LitigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequestArbitratorRecusalRequest.LitigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequestArbitratorRecusalRequest.LitigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequestArbitratorRecusalRequest.LitigationRequest.Contract.SecondPartyCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequestArbitratorRecusalRequest.LitigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.RequestParty = litigationRequestArbitratorRecusalRequest.RequestParty;



                    if (litigationRequestArbitratorRecusalRequest.ApproveRejectStatus.HasValue)
                    {
                        litigationRequestMircoDto.Response = litigationRequestArbitratorRecusalRequest.ApproveRejectStatus.Value;
                        litigationRequestMircoDto.IsResponseDone = "--";
                    }
                    else
                    {
                        litigationRequestMircoDto.Response = ApproveRejectStatus.NoResponse;
                        litigationRequestMircoDto.IsResponseDone = "تم";
                    }

                    litigationRequestMircoDto.Status = litigationRequestArbitratorRecusalRequest.LitigationRequest.InternalStatus.RequestStatus;
                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                }
            }
            return new DataResponse<PagedResult<LitigationRequestMircoExplanationRequestDto>>
            {
                Data =
                  new PagedResult<LitigationRequestMircoExplanationRequestDto>
                  {
                      Results = litigationRequestMircoDtos,
                      CurrentPage = litigationRequestArbitratorRecusalRequests.CurrentPage,
                      PageCount = litigationRequestArbitratorRecusalRequests.PageCount,
                      PageSize = litigationRequestArbitratorRecusalRequests.PageSize,
                      TotalCount = litigationRequestArbitratorRecusalRequests.TotalCount
                  },
                IsDone = true
            };


        }
        public async Task<BaseResponse> GetLitigationRequestForArbitratorRecusalRequestForArbitratorByRequestId(Guid requestId)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var requestArbitrationReportRecusalRequest = (await _repository.GetQueryable<LitigationRequestArbitratorRecusalRequest, Guid>())
                         .Include(x => x.ArbitratorCitizen)
                         .Include(x => x.LitigationRequestArbitratorRecusalRequestFiles)
                         .ThenInclude(x => x.RecusalRequestFile)
                          .Include(x => x.LitigationRequest)
                         .ThenInclude(x => x.ApplicantCitizen)
                         .Include(x => x.LitigationRequest)
                         .ThenInclude(x => x.Area)
                        .Include(x => x.LitigationRequest)

                         .ThenInclude(x => x.City)
                         .Include(x => x.LitigationRequest)

                         .ThenInclude(x => x.Contract)
                         .ThenInclude(x => x.FirstPartyFacility)
                         .Include(x => x.LitigationRequest)
                         .ThenInclude(x => x.Contract)
                         .ThenInclude(x => x.FirstPartyCitizen)
                         .Include(x => x.LitigationRequest)
                         .ThenInclude(x => x.Contract)
                         .ThenInclude(x => x.SecondPartyFacility)
                         .Include(x => x.LitigationRequest)
                         .ThenInclude(x => x.Contract)
                         .ThenInclude(x => x.SecondPartyCitizen)

                         .Include(x => x.LitigationRequest)
                         .ThenInclude(x => x.Contract)
                         .ThenInclude(x => x.LitigationRequestContractType)
                         .Where(x => x.Id == requestId).AsNoTracking().ToList().FirstOrDefault();



            if (requestArbitrationReportRecusalRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };

            bool isDone = false;
            bool allowAdd = true;

            var request = _mapper.Map<LitigationRequestShortDto>(requestArbitrationReportRecusalRequest.LitigationRequest);

            LitigationRequestParty litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.None;


            if (requestArbitrationReportRecusalRequest.LitigationRequest.IsFinished.HasValue)
            {
                if (requestArbitrationReportRecusalRequest.LitigationRequest.IsFinished.Value)
                {
                    allowAdd = false;
                    isDone = true;
                }
            }

            if (!isDone)
            {
                if (requestArbitrationReportRecusalRequest.ApproveRejectStatus.HasValue)
                    allowAdd = false;
            }
            var reportExplanationRequest = _mapper.Map<LitigationRequestArbitratorRecusalRequestGetDto>(requestArbitrationReportRecusalRequest);
            return new DataResponse<LitigationRequestForArbitratorRecusalRequestDto>()
            {
                Data = new LitigationRequestForArbitratorRecusalRequestDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    request = reportExplanationRequest
                },
                IsDone = true,
            };
        }
        public async Task<BaseResponse> AddArbitrationReportArbitratorRecusalForArbitrator(LitigationRequestArbitrationRecusalRequestResponseDto requestPayload)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var arbitrationReportExplanationRequest = (await _repository.GetQueryable<LitigationRequestArbitratorRecusalRequest, Guid>())
                     .Include(x => x.LitigationRequest)
                     .Include(x => x.LitigationRequest.ApplicantCitizen)
                    .Include(x => x.LitigationRequest.Contract)
                    .Include(x => x.LitigationRequest.Contract.FirstPartyCitizen)
                    .Include(x => x.LitigationRequest.Contract.SecondPartyCitizen)
                    .Include(x => x.LitigationRequest.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                    .Include(x => x.LitigationRequest.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                     .FirstOrDefault(x => x.Id == requestPayload.Id);

                if (arbitrationReportExplanationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                arbitrationReportExplanationRequest.ResponseOn = DateTime.Now;
                arbitrationReportExplanationRequest.ApproveRejectStatus = requestPayload.Response;

                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;
                try
                {
                    var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                    if (arbitrationAdmin.IsDone)
                    {

                        var msg = "";
                        var arbitratorInfo = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(c => c.Id == arbitrationReportExplanationRequest.ArbitratorCitizenId);
                        var litigationRequest = arbitrationReportExplanationRequest.LitigationRequest;
                        switch (requestPayload.Response)
                        {

                            case ApproveRejectStatus.Approve:
                                // Send Notification
                                // تم الموافقة على طلب رد محكم
                                msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitraorRecusal_AddRequest_Approve).Result.Data?.Value;
                                break;
                            case ApproveRejectStatus.Reject:
                                // Send Notification
                                // تم رفض طلب رد محكم
                                msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitraorRecusal_AddRequest_Reject).Result.Data?.Value;
                                break;
                        }
                        msg = msg?.Replace("{ArbitratorName}", arbitratorInfo?.FullName);
                        msg = msg?.Replace("{LitigationCode}", arbitrationReportExplanationRequest.LitigationRequest?.RequestCode);
                        List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                        new SysNotificationDto{
                        ActorId = litigationRequest.ApplicantCitizen.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Plaintiff,
                        Message=msg
                        },
                           new SysNotificationDto{
                        ActorId = (Guid)arbitrationAdmin.Data?.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Employee,
                        Message=msg
                        }

                        };
                        List<string> emails = new List<string>() {
                                litigationRequest.ApplicantCitizen.Email,
                                  arbitrationAdmin.Data?.Email
                                };
                        if (litigationRequest?.Contract?.FirstPartyCitizenId != null && litigationRequest?.Contract?.FirstPartyCitizenId != litigationRequest.ApplicantCitizenId)
                        {
                            notifications.Add(new SysNotificationDto
                            {
                                ActorId = (Guid)litigationRequest.Contract.FirstPartyCitizenId,
                                RequestId = litigationRequest.Id,
                                CreatedOn = DateTime.Now,
                                RequestTypeId = RequestType.LitigationRequest,
                                ActorTypeId = NotificationActorType.Plaintiff,
                                Message = msg
                            });
                            emails.Add(litigationRequest.Contract.FirstPartyCitizen.Email);


                        };
                        if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility.CitizenId != litigationRequest.ApplicantCitizenId)
                        {
                            notifications.Add(new SysNotificationDto
                            {
                                ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.CitizenId,
                                RequestId = litigationRequest.Id,
                                CreatedOn = DateTime.Now,
                                RequestTypeId = RequestType.LitigationRequest,
                                ActorTypeId = NotificationActorType.Plaintiff,
                                Message = msg
                            });
                            emails.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Email);
                        };
                        if (litigationRequest?.Contract?.SecondPartyCitizenId != null)
                        {
                            notifications.Add(new SysNotificationDto
                            {
                                ActorId = (Guid)litigationRequest.Contract.SecondPartyCitizenId,
                                RequestId = litigationRequest.Id,
                                CreatedOn = DateTime.Now,
                                RequestTypeId = RequestType.LitigationRequest,
                                ActorTypeId = NotificationActorType.Defendant,
                                Message = msg
                            });
                            emails.Add(litigationRequest.Contract.SecondPartyCitizen.Email);
                        };
                        if (litigationRequest?.Contract?.SecondPartyFacilityId != null)
                        {
                            notifications.Add(new SysNotificationDto
                            {
                                ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.CitizenId,
                                RequestId = litigationRequest.Id,
                                CreatedOn = DateTime.Now,
                                RequestTypeId = RequestType.LitigationRequest,
                                ActorTypeId = NotificationActorType.Defendant,
                                Message = msg
                            });
                            emails.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Email);
                        };
                        _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                        {
                            To = emails,
                            Subject = "طلب رد محكم",
                            Body = msg
                        }, null, notifications);

                    }
                }
                catch (Exception e)
                {


                }


                var citizen = (await _repository.GetQueryable<Citizen, Guid>()).Where(x => x.Id == arbitrationReportExplanationRequest.ArbitratorCitizenId).FirstOrDefault();
                try
                {
                    switch (requestPayload.Response)
                    {
                        case ApproveRejectStatus.Approve:
                            // تم الموافقة على طلب رد محكم
                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = arbitrationReportExplanationRequest.LitigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم فبول طلب رد المحكم " + citizen.FullName + " من قبل " + ((arbitrationReportExplanationRequest.RequestParty == LitigationRequestParty.Plaintiff) ? "المدعى" : "المدعى عليه")
                            });
                            break;
                        case ApproveRejectStatus.Reject:
                            // تم رفض طلب رد محكم
                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = arbitrationReportExplanationRequest.LitigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم رفض طلب رد المحكم " + citizen.FullName + " من قبل " + ((arbitrationReportExplanationRequest.RequestParty == LitigationRequestParty.Plaintiff) ? "المدعى" : "المدعى عليه")
                            });
                            break;
                    }
                }
                catch (Exception)
                {

                    throw;
                }


                saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

            }
            catch (Exception ex)
            {
                new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
            return await GetLitigationRequestForArbitratorRecusalRequestForArbitratorByRequestId(requestPayload.LitigationRequestId);
        }
        #endregion
    }
}
