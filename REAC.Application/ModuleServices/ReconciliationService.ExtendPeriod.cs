using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.ModuleServices
{
    public partial class ReconciliationService
    {

        #region Extend Period Request
        public async Task<BaseResponse> AddReconciliationExtendPeriod(LitigationRequestExtendPeriodRequestCreateUpdateDto requestPayload, CancellationToken cancellationToken)
        {
            var msg = "";
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Where(x => x.Id == requestPayload.LitigationRequestId).ToList().FirstOrDefault();
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                       .Include(x => x.LitigationRequestExtendPeriodRequests.Where(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation))
                       .Where(x => x.Id == requestPayload.LitigationRequestId & x.ReformerCitizenId == referenceId)
                       .Single();

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                bool allowAdd = true;

                if (litigationRequest.LitigationRequestExtendPeriodRequests.Count() > 0)
                    allowAdd = false;

                if (allowAdd)
                {
                    LitigationRequestExtendPeriodRequest litigationRequestExtendPeriodRequest = new LitigationRequestExtendPeriodRequest()
                    {
                        Id = requestPayload.LitigationRequestId,
                        LitigationRequestId = requestPayload.LitigationRequestId,
                        RequiredDays = requestPayload.RequiredDays,
                        RequestReason = requestPayload.RequestReason,
                        LitigationRequestExtendPeriodRequestTypeId = LitigationRequestExtendPeriodRequestType.Reconciliation
                    };

                    if (requestPayload.Files != null)
                    {
                        requestPayload.Files.ToList().ForEach(t =>
                        {
                            litigationRequestExtendPeriodRequest.LitigationRequestExtendPeriodRequestFiles.Add(new LitigationRequestExtendPeriodRequestFile()
                            {
                                Id = Guid.NewGuid(),
                                FileManagerId = t,
                            });


                        });
                    }

                    await _repository.AddAsync<LitigationRequestExtendPeriodRequest, Guid>(litigationRequestExtendPeriodRequest);

                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationRequest.Id,
                        ShowForApplicant = false,
                        LogMessage = "تم إضافة طلب مد مدة الصلح"
                    });

                    var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;


                    // Send Notification
                    // تم إرسال طلب مد مدة الصلح
                    try
                    {
                        msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ExtendRequest_Added).Result.Data?.Value;
                        msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                        SendNotificationMSWithAllTypes(msg, litigationRequest, "Citizen Litigation Request For Report Reply Added");
                    }
                    catch (Exception e)
                    {


                    }

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
            return await GetLitigationRequestExtendPeriod(requestPayload.LitigationRequestId, cancellationToken);
        }
        public async Task<BaseResponse> AddReconciliationExtendPeriodForEmployee(LitigationRequestExtendPeriodRequestCreateUpdateEmployeeDto requestPayload, CancellationToken cancellationToken)
        {
            var msg = "";
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Where(x => x.Id == requestPayload.LitigationRequestId).ToList().FirstOrDefault();
            try
            {
                var litigationRequestExtendPeriodRequest = (await _repository.GetQueryable<LitigationRequestExtendPeriodRequest, Guid>())?
                    .Include(x => x.LitigationRequest)
                        .Where(x => x.LitigationRequestId == requestPayload.LitigationRequestId & x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation)
                        .SingleOrDefault();


                if (litigationRequestExtendPeriodRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };
                litigationRequestExtendPeriodRequest.LitigationRequestExtendPeriodRequestStatusId = requestPayload.LitigationRequestExtendPeriodRequestStatusId;
                litigationRequestExtendPeriodRequest.FinishStatusReason = requestPayload.FinishStatusReason;

                var savePeriodResponse = await _repository.SaveChangesAsync();
                if (!savePeriodResponse.IsDone)
                    return savePeriodResponse;

                //var updateResponse = _repository.Update<LitigationRequestExtendPeriodRequest, Guid>(litigationRequestExtendPeriodRequest);
                //if (!updateResponse.IsDone)
                //    return updateResponse;

                if (requestPayload.LitigationRequestExtendPeriodRequestStatusId == LitigationRequestExtendPeriodRequestStatus.Approve)
                {
                    litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                      .Where(x => x.Id == requestPayload.LitigationRequestId)
                      .SingleOrDefault();

                    litigationRequest.ReconciliationPeriod += litigationRequestExtendPeriodRequest.RequiredDays;

                    savePeriodResponse = await _repository.SaveChangesAsync();
                    if (!savePeriodResponse.IsDone)
                        return savePeriodResponse;
                }
                switch (requestPayload.LitigationRequestExtendPeriodRequestStatusId)
                {
                    case LitigationRequestExtendPeriodRequestStatus.Approve:
                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequestExtendPeriodRequest.LitigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم قبول طلب مد مدة الصلح"
                        });
                        // Send Notification
                        // تم قبول طلب مد مدة الصلح
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_ExtendRequest_Accepted).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            SendNotificationMSWithAllTypes(msg, litigationRequest, "Citizen Litigation Request For Report Reply Added");
                        }
                        catch (Exception e)
                        {


                        }
                        break;
                    case LitigationRequestExtendPeriodRequestStatus.Reject:

                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequestExtendPeriodRequest.LitigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم رفض طلب مد مدة الصلح"
                        });

                        // Send Notification
                        // تم رفض طلب مد مدة الصلح
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ExtendRequest_Rejected).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            SendNotificationMSWithAllTypes(msg, litigationRequest, "Citizen Litigation Request For Report Reply Added");
                        }
                        catch (Exception e)
                        {
                        }
                        break;
                }

                savePeriodResponse = await _repository.SaveChangesAsync();
                if (!savePeriodResponse.IsDone)
                    return savePeriodResponse;

                return await GetLitigationRequestExtendPeriodForEmployee(requestPayload.LitigationRequestId, cancellationToken);
            }
            catch (Exception ex)
            {
                new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
            return await GetLitigationRequestExtendPeriod(requestPayload.LitigationRequestId, cancellationToken);
        }

        public async Task<BaseResponse> GetLitigationRequestExtendPeriod(Guid requestId, CancellationToken cancellationToken)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.Area)
                         .Include(x => x.City)
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
                         .Where(x => x.Id == requestId).ToList().FirstOrDefault();

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            var extendPeriodRequest = _mapper.Map<List<LitigationRequestExtendPeriodRequestGetDto>>(litigationRequest?.LitigationRequestExtendPeriodRequests);

            bool isDone = false;
            bool allowAdd = true;

            if (litigationRequest.IsFinished.HasValue)
            {
                if (litigationRequest.IsFinished.Value)
                {
                    allowAdd = false;
                    isDone = true;
                }
            }
            if (!isDone)
            {
                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Reconciliation)
                    allowAdd = false;
                else
                {
                    if (litigationRequest?.LitigationRequestExtendPeriodRequests != null)
                    {
                        if (litigationRequest.LitigationRequestExtendPeriodRequests.Count(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation) > 0)
                            allowAdd = false;
                    }
                }
            }

            return new DataResponse<LitigationRequestForExtendPeriodRequestDto>()
            {
                Data = new LitigationRequestForExtendPeriodRequestDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    LitigationRequestExtendPeriodRequestCreateUpdateDto = extendPeriodRequest.FirstOrDefault()
                },
                IsDone = true,
                Message = "تمت العملية بنجاح"
            };

        }
        public async Task<BaseResponse> GetLitigationRequestExtendPeriodForEmployee(Guid requestId, CancellationToken cancellationToken)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.Area)
                         .Include(x => x.City)
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
                         .Where(x => x.Id == requestId).ToList().FirstOrDefault();

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            var extendPeriodRequest = _mapper.Map<List<LitigationRequestExtendPeriodRequestGetDto>>(litigationRequest?.LitigationRequestExtendPeriodRequests.Where(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation));

            bool isDone = false;
            bool allowAdd = true;

            if (litigationRequest.IsFinished.HasValue)
            {
                if (litigationRequest.IsFinished.Value)
                {
                    allowAdd = false;
                    isDone = true;
                }
            }
            if (!isDone)
            {
                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Reconciliation)
                    allowAdd = false;
                else
                {
                    if (litigationRequest?.LitigationRequestExtendPeriodRequests != null)
                    {
                        if (litigationRequest.LitigationRequestExtendPeriodRequests.Count > 0)
                        {
                            if (litigationRequest.LitigationRequestExtendPeriodRequests.FirstOrDefault()?.LitigationRequestExtendPeriodRequestStatusId != null)
                                allowAdd = false;
                        }
                    }
                }
            }

            return new DataResponse<LitigationRequestForExtendPeriodRequestDto>()
            {
                Data = new LitigationRequestForExtendPeriodRequestDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    LitigationRequestExtendPeriodRequestCreateUpdateDto = extendPeriodRequest.FirstOrDefault()
                },
                IsDone = true,
            };

        }

        public async Task<BaseResponse> GetLitigationRequestExtendPeriod(CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
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
                                      .Include(x => x.Contract.LitigationRequestContractType)
                                      .Include(x => x.Contract.ContractFile)
                                      .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                                      .Include(x => x.Contract.FoundingLetterFile)
                                      .Include(x => x.Contract.InternalRegulationsFile)
                                      .Include(x => x.Contract.FirstPartyFacility)
                                      .Include(x => x.Contract.FirstPartyCitizen)
                                      .Include(x => x.Contract.SecondPartyFacility)
                                      .Include(x => x.Contract.SecondPartyCitizen).Include(x => x.Contract.LitigationRequestContractType)

                                      .Where(x => x.ReformerCitizenId == referenceId
                                      & x.RequestCode != null & x.IsFinished != true
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration
                                      & x.LitigationRequestStage == LitigationRequestStage.Reconciliation
                                      | x.LitigationRequestExtendPeriodRequests.Where(x => x.LitigationRequestExtendPeriodRequestTypeId
                                      == LitigationRequestExtendPeriodRequestType.Reconciliation).Count() > 0
                                      ).ToList();

            List<LitigationRequestMircoDto> litigationRequestMircoDtos = new List<LitigationRequestMircoDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests)
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
                    if (litigationRequest.IsFinished.HasValue)
                    {
                        if (litigationRequest.IsFinished.Value)
                        {
                            allowAdd = false;
                            isDone = true;
                        }
                    }
                    if (!isDone)
                    {
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Reconciliation)
                            allowAdd = false;
                        else
                        {
                            if (litigationRequest?.LitigationRequestExtendPeriodRequests != null)
                            {
                                if (litigationRequest.LitigationRequestExtendPeriodRequests.Count(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation) > 0)
                                    allowAdd = false;
                            }
                        }
                    }

                    if (allowAdd)
                        litigationRequestMircoDto.IsDone = "--";
                    else
                        litigationRequestMircoDto.IsDone = "تم";

                    litigationRequestMircoDto.Status = litigationRequest.InternalStatus.RequestStatus;
                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);

                }
            }

            return new DataResponse<List<LitigationRequestMircoDto>>()
            {
                Data = litigationRequestMircoDtos,
                IsDone = true
            };
        }
        public async Task<BaseResponse> GetLitigationRequestExtendPeriodForEmployee(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {

            PagedResult<LitigationRequest> litigationRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
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
                                      .Include(x => x.Contract.LitigationRequestContractType)
                                      .Include(x => x.Contract.ContractFile)
                                      .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                                      .Include(x => x.Contract.FoundingLetterFile)
                                      .Include(x => x.Contract.InternalRegulationsFile)
                                      .Include(x => x.Contract.FirstPartyFacility)
                                      .Include(x => x.Contract.FirstPartyCitizen)
                                      .Include(x => x.Contract.SecondPartyFacility)
                                      .Include(x => x.Contract.SecondPartyCitizen).Include(x => x.Contract.LitigationRequestContractType);


            litigationRequests = entitiesQuerable.Where(x => x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected & x.LitigationRequestExtendPeriodRequests.Count() > 0)
                                       .OrderByDescending(x => x.CreatedOn)
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
                    if (litigationRequest.IsFinished.HasValue)
                    {
                        if (litigationRequest.IsFinished.Value)
                        {
                            allowAdd = false;
                            isDone = true;
                        }
                    }
                    if (!isDone)
                    {
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Reconciliation)
                            allowAdd = false;
                        else
                        {
                            if (litigationRequest?.LitigationRequestExtendPeriodRequests != null)
                            {
                                if (litigationRequest.LitigationRequestExtendPeriodRequests.Count(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation) > 0)
                                {
                                    if (litigationRequest.LitigationRequestExtendPeriodRequests.FirstOrDefault()?.LitigationRequestExtendPeriodRequestStatusId != null)
                                        allowAdd = false;
                                }
                                else
                                {
                                    allowAdd = false;
                                }
                            }
                            else
                            {
                                allowAdd = false;
                            }
                        }
                    }

                    if (allowAdd)
                        litigationRequestMircoDto.IsDone = "--";
                    else
                        litigationRequestMircoDto.IsDone = "تم";

                    litigationRequestMircoDto.Status = litigationRequest?.InternalStatus?.RequestStatus;
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

        #endregion

    }
}
