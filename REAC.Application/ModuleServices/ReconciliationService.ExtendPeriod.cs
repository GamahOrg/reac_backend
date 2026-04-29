using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities;
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
            LitigationRequest litigationRequest = null;
            var msg = "";
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                              .Include(x => x.Area)
                              .Include(x => x.City)
                              .Include(x => x.ApplicantCitizen)
                              .Include(x => x.ReformerCitizen)
                              .Include(x => x.Contract)
                              .Include(x => x.Contract.FirstPartyFacility)
                              .Include(x => x.Contract.FirstPartyCitizen)
                              .Include(x => x.Contract.SecondPartyFacility)
                              .Include(x => x.Contract.SecondPartyCitizen)
                              .Include(x => x.LitigationRequestExtendPeriodRequests.Where(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation))
                              .FirstOrDefault(x => x.Id == requestPayload.LitigationRequestId && x.ReformerCitizenId == referenceId);

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                bool allowAdd = true;

                if (litigationRequest.LitigationRequestExtendPeriodRequests != null && litigationRequest.LitigationRequestExtendPeriodRequests.Any(x => x.LitigationRequestExtendPeriodRequestStatusId == null))
                    allowAdd = false;

                if (allowAdd)
                {
                    LitigationRequestExtendPeriodRequest litigationRequestExtendPeriodRequest = new LitigationRequestExtendPeriodRequest()
                    {
                        Id = Guid.NewGuid(),
                        LitigationRequestId = requestPayload.LitigationRequestId,
                        RequiredDays = requestPayload.RequiredDays,
                        RequestReason = requestPayload.RequestReason,
                        CreatedOn = DateTime.Now,
                        CreatedBy = referenceId,
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
                        msg = "تم طلب مد مدة الصلح للدعوى رقم " + litigationRequest?.RequestCode;
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        string body;
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_ExtendRequest_Added).Result.Data?.Value);
                            body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                        }
                       await SendNotificationMSForReconciliationDeptOnly(body, msg, litigationRequest, "طلب مد مدة الصلح");
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
                          .Include(x => x.ReformerCitizen)
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
                        .Where(x => x.LitigationRequestId == requestPayload.LitigationRequestId 
                                & x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation
                                & x.LitigationRequestExtendPeriodRequestStatusId == null)
                        .OrderByDescending(x => x.CreatedOn)
                        .FirstOrDefault();


                if (litigationRequestExtendPeriodRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };
                litigationRequestExtendPeriodRequest.LitigationRequestExtendPeriodRequestStatusId = requestPayload.LitigationRequestExtendPeriodRequestStatusId;
                litigationRequestExtendPeriodRequest.FinishStatusReason = requestPayload.FinishStatusReason;
                litigationRequestExtendPeriodRequest.Notes = requestPayload.Notes;

                // Check on Reformer DeActivation Period
                //var reformerStopDecision = (await _repository.GetQueryable<ReformerStopFreeUpDecision, int>())
                //    .OrderByDescending(y => y.DecisionDate)
                //    .AsNoTracking()
                //    .FirstOrDefault(x => x.CitizenId == litigationRequestExtendPeriodRequest.CreatedBy && x.ReformerStatusId == ReformerStatus.Stopped && x.DecisionTo != null);

                //if(reformerStopDecision != null)
                //{

                //    var extendRequestDate = DateTime.Now.AddDays(litigationRequestExtendPeriodRequest.RequiredDays);

                //    if(extendRequestDate <= reformerStopDecision.DecisionTo && extendRequestDate >= reformerStopDecision.DecisionFrom)
                //    {

                //    }
                //}



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
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            string body;
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_ExtendRequest_Accepted).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            await SendNotificationMSWithAllTypes(body, msg, litigationRequest, "Citizen Litigation Request For Report Reply Added");
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
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            string body;
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ExtendRequest_Rejected).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            await SendNotificationMSWithAllTypes(body, msg, litigationRequest, "Citizen Litigation Request For Report Reply Added");
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
                         .Include(x => x.DefendantCitizen)
                         .Include(x => x.DefendantFacility)
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
                         .Where(x => x.Id == requestId).ToList().LastOrDefault();

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            var extendPeriodRequest = _mapper.Map<List<LitigationRequestExtendPeriodRequestGetDto>>(litigationRequest?.LitigationRequestExtendPeriodRequests
                                .OrderByDescending(x => x.CreatedOn));
            
            // Create a SEPARATE instance for the current/new request to avoid corrupting history references
            var extendPeriodRequestDto = new LitigationRequestExtendPeriodRequestGetDto();

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
                        if (litigationRequest.LitigationRequestExtendPeriodRequests.Any(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation
                            && x.LitigationRequestExtendPeriodRequestStatusId == null))
                            allowAdd = false;
                    }
                }
            }

            if (extendPeriodRequestDto == null)
            {
                extendPeriodRequestDto = new LitigationRequestExtendPeriodRequestGetDto();
            }

            if (litigationRequest != null)
            {
                var approvedDaysInHistory = (await _repository.GetQueryable<LitigationRequestExtendPeriodRequest, Guid>())
                    .Where(x => x.LitigationRequestId == litigationRequest.Id
                             && x.LitigationRequestExtendPeriodRequestStatusId == LitigationRequestExtendPeriodRequestStatus.Approve
                             && x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation)
                    .Sum(x => x.RequiredDays);

                var currentTotalPeriod = litigationRequest.ReconciliationPeriod.GetValueOrDefault(0);
                var originalDuration = currentTotalPeriod - approvedDaysInHistory;

                var baseDate = litigationRequest.StartReconciliationDate ?? litigationRequest.CreatedOn;
                var orderedHistory = extendPeriodRequest.OrderBy(x => x.CreatedOn).ToList();
                var currentCumulativeDays = originalDuration;

                foreach (var h in orderedHistory)
                {
                    h.LitigationFinishedDate = baseDate.AddDays(currentCumulativeDays);
                    h.LitigationFininshedNewDate = h.LitigationFinishedDate.Value.AddDays(h.RequiredDays);

                    if (h.LitigationRequestExtendPeriodRequestStatusId == (int)LitigationRequestExtendPeriodRequestStatus.Approve)
                    {
                        currentCumulativeDays += h.RequiredDays;
                    }
                }

                if (extendPeriodRequestDto != null)
                {
                    var lastHistory = orderedHistory.LastOrDefault();

                    if (lastHistory == null || lastHistory.LitigationRequestExtendPeriodRequestStatusId != null)
                    {
                        extendPeriodRequestDto.LitigationFinishedDate = baseDate.AddDays(currentCumulativeDays);
                        extendPeriodRequestDto.LitigationFininshedNewDate = extendPeriodRequestDto.LitigationFinishedDate.Value.AddDays(extendPeriodRequestDto.RequiredDays);
                    }

                    if (lastHistory != null && lastHistory.LitigationRequestExtendPeriodRequestStatusId == null)
                    {
                         extendPeriodRequestDto.Status = "extendLitigationRequest.underReview";
                         extendPeriodRequestDto.FinishStatusReason = lastHistory.FinishStatusReason;
                    }
                }
            }

            return new DataResponse<LitigationRequestForExtendPeriodRequestDto>()
            {
                Data = new LitigationRequestForExtendPeriodRequestDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    LitigationRequestExtendPeriodRequestCreateUpdateDto = extendPeriodRequestDto,
                    LitigationRequestExtendPeriodRequests = extendPeriodRequest
                },
                IsDone = true,
                Message = "تمت العملية بنجاح"
            };

        }
        public async Task<BaseResponse> GetLitigationRequestExtendPeriodForEmployee(Guid requestId, CancellationToken cancellationToken)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.Area)
                         .Include(x => x.LitigationRequestCategory)
                         .Include(x => x.City)
                         .Include(x => x.LitigationRequestExtendPeriodRequests.Where(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation))
                         .ThenInclude(x => x.LitigationRequestExtendPeriodRequestFiles)
                         .ThenInclude(x => x.FileManager)
                         .Include(x => x.ApplicantCitizen)
                         .Include(x => x.DefendantCitizen)
                         .Include(x => x.DefendantFacility)
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
                         .Where(x => x.LitigationRequestExtendPeriodRequests.Any(s=>s.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation))
                         .Where(x => x.Id == requestId).ToList().FirstOrDefault();

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            var extendPeriodRequest = _mapper.Map<List<LitigationRequestExtendPeriodRequestGetDto>>(litigationRequest?.LitigationRequestExtendPeriodRequests
                                .Where(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation)
                                .OrderByDescending(x => x.CreatedOn));

            // Create a SEPARATE instance for the current/new request to avoid corrupting history references
            var extendPeriodRequestDto = new LitigationRequestExtendPeriodRequestGetDto();
            
            bool allowAdd = true;

            if (litigationRequest.IsFinished.HasValue && litigationRequest.IsFinished.Value)
            {
                allowAdd = false;
            }
            else if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Reconciliation)
            {
                allowAdd = false;
            }
            else if (litigationRequest?.LitigationRequestExtendPeriodRequests != null)
            {
                var extendPeriodEntity = litigationRequest.LitigationRequestExtendPeriodRequests
                    .Where(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation)
                    .OrderByDescending(x => x.CreatedOn)
                    .FirstOrDefault();

                if (extendPeriodEntity != null)
                {
                    if (extendPeriodEntity.LitigationRequestExtendPeriodRequestStatusId != null)
                        allowAdd = false;
                }
            }

            // (Instance already created above to isolate from history references)

            if (litigationRequest != null)
            {
                var approvedDaysInHistory = litigationRequest.LitigationRequestExtendPeriodRequests
                    .Where(x => x.LitigationRequestExtendPeriodRequestStatusId == LitigationRequestExtendPeriodRequestStatus.Approve
                             && x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation)
                    .Sum(x => x.RequiredDays);

                var originalDuration = litigationRequest.ReconciliationPeriod.GetValueOrDefault(0) - approvedDaysInHistory;

                var baseDate = litigationRequest.StartReconciliationDate ?? litigationRequest.CreatedOn;
                var orderedHistory = extendPeriodRequest.OrderBy(x => x.CreatedOn).ToList();
                var currentCumulativeDays = originalDuration;

                foreach (var h in orderedHistory)
                {
                    h.LitigationFinishedDate = baseDate.AddDays(currentCumulativeDays);
                    h.LitigationFininshedNewDate = h.LitigationFinishedDate.Value.AddDays(h.RequiredDays);

                    if (h.LitigationRequestExtendPeriodRequestStatusId == (int)LitigationRequestExtendPeriodRequestStatus.Approve)
                    {
                        currentCumulativeDays += h.RequiredDays;
                    }
                }

                if (extendPeriodRequestDto != null)
                {
                    var lastHistory = orderedHistory.LastOrDefault();

                    if (lastHistory == null || lastHistory.LitigationRequestExtendPeriodRequestStatusId != null)
                    {
                        extendPeriodRequestDto.LitigationFinishedDate = baseDate.AddDays(currentCumulativeDays);
                        extendPeriodRequestDto.LitigationFininshedNewDate = extendPeriodRequestDto.LitigationFinishedDate.Value.AddDays(extendPeriodRequestDto.RequiredDays);
                    }

                    if (lastHistory != null && lastHistory.LitigationRequestExtendPeriodRequestStatusId == null)
                    {
                        extendPeriodRequestDto.Status = "extendLitigationRequest.underReview";
                        extendPeriodRequestDto.FinishStatusReason = lastHistory.FinishStatusReason;
                    }
                }
            }

            return new DataResponse<LitigationRequestForExtendPeriodRequestDto>()
            {
                Data = new LitigationRequestForExtendPeriodRequestDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    LitigationRequestExtendPeriodRequestCreateUpdateDto = extendPeriodRequestDto,
                    LitigationRequestExtendPeriodRequests = extendPeriodRequest
                },
                IsDone = true,
            };

        }

        public async Task<BaseResponse> GetLitigationRequestExtendPeriod(CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                                      .Include(x => x.LitigationRequestExtendPeriodRequests.Where(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation))
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
                                      .Include(x => x.Contract.SecondPartyCitizen)
                                      .Include(x => x.Contract.LitigationRequestContractType)
                                      .Where(x => (x.ReformerCitizenId == referenceId
                                                   && x.RequestCode != null 
                                                   && x.IsFinished != true
                                                   && x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration
                                                   && x.LitigationRequestStage == LitigationRequestStage.Reconciliation)
                                                || x.LitigationRequestExtendPeriodRequests.Any(r => r.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation)
                                      )
                                      .OrderByDescending(x => x.CreatedOn)
                                      .AsSplitQuery()
                                      .AsNoTracking()
                                      .ToList();

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
                    var extendPeriodRequest = litigationRequest?.LitigationRequestExtendPeriodRequests
                        ?.OrderByDescending(x => x.CreatedOn)
                        .FirstOrDefault(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation);

                    if (extendPeriodRequest == null)
                        litigationRequestMircoDto.IsDone = "--";
                    else if (extendPeriodRequest.LitigationRequestExtendPeriodRequestStatusId == null)
                        litigationRequestMircoDto.IsDone = "extendLitigationRequest.underReview";
                    else if (extendPeriodRequest.LitigationRequestExtendPeriodRequestStatusId == LitigationRequestExtendPeriodRequestStatus.Approve)
                        litigationRequestMircoDto.IsDone = "extendLitigationRequest.accepted";
                    else if (extendPeriodRequest.LitigationRequestExtendPeriodRequestStatusId == LitigationRequestExtendPeriodRequestStatus.Reject)
                        litigationRequestMircoDto.IsDone = "extendLitigationRequest.rejected";

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
                                  .Include(x => x.LitigationRequestExtendPeriodRequests.Where(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation))
                                      .ThenInclude(x => x.LitigationRequestExtendPeriodRequestFiles)
                                      .ThenInclude(x => x.FileManager)
                                      .Include(x => x.ApplicantCitizen)
                                      .Include(x => x.DefendantCitizen)
                                      .Include(x => x.DefendantFacility)
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
                                      .Where(x => x.LitigationRequestExtendPeriodRequests.Any(s => s.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation));


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
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.DefendantCitizen == null) ? litigationRequest.DefendantFacility?.ArabicFacilityName : litigationRequest.DefendantCitizen?.FullName;
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
                                if (litigationRequest.LitigationRequestExtendPeriodRequests.Any(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation && x.LitigationRequestExtendPeriodRequestStatusId == null))
                                    allowAdd = false;
                            }
                        }
                    }

                    var extendPeriodRequest = litigationRequest?.LitigationRequestExtendPeriodRequests
                        ?.Where(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation)
                        ?.OrderByDescending(x => x.CreatedOn)
                        .FirstOrDefault();

                    if (extendPeriodRequest == null)
                        litigationRequestMircoDto.IsDone = "--";
                    else if (extendPeriodRequest.LitigationRequestExtendPeriodRequestStatusId == null)
                        litigationRequestMircoDto.IsDone = "extendLitigationRequest.underReview";
                    else if (extendPeriodRequest.LitigationRequestExtendPeriodRequestStatusId == LitigationRequestExtendPeriodRequestStatus.Approve)
                        litigationRequestMircoDto.IsDone = "extendLitigationRequest.accepted";
                    else if (extendPeriodRequest.LitigationRequestExtendPeriodRequestStatusId == LitigationRequestExtendPeriodRequestStatus.Reject)
                        litigationRequestMircoDto.IsDone = "extendLitigationRequest.rejected";

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

        public async Task<BaseResponse> GetPagedLitigationRequestExtendPeriod(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                  .Include(x => x.LitigationRequestExtendPeriodRequests.Where(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation))
                                      .ThenInclude(x => x.LitigationRequestExtendPeriodRequestFiles)
                                      .ThenInclude(x => x.FileManager)
                                      .Include(x => x.ApplicantCitizen)
                                      .Include(x => x.DefendantCitizen)
                                      .Include(x => x.DefendantFacility)
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
                                      .Where(x => (x.ReformerCitizenId == referenceId
                                                   || x.ApplicantCitizenId == referenceId
                                                   || x.DefendantCitizenId == referenceId)
                                                   && x.RequestCode != null 
                                                   && x.IsFinished != true
                                                   && x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration
                                                   && x.LitigationRequestStage == LitigationRequestStage.Reconciliation
                                      );


            litigationRequests = entitiesQuerable
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
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility?.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen?.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.DefendantCitizen == null) ? litigationRequest.DefendantFacility?.ArabicFacilityName : litigationRequest.DefendantCitizen?.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;

                    var extendPeriodRequest = litigationRequest?.LitigationRequestExtendPeriodRequests
                        ?.OrderByDescending(x => x.CreatedOn)
                        .FirstOrDefault(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Reconciliation);

                    if (extendPeriodRequest == null)
                        litigationRequestMircoDto.IsDone = "--";
                    else if (extendPeriodRequest.LitigationRequestExtendPeriodRequestStatusId == null)
                        litigationRequestMircoDto.IsDone = "extendLitigationRequest.underReview";
                    else if (extendPeriodRequest.LitigationRequestExtendPeriodRequestStatusId == LitigationRequestExtendPeriodRequestStatus.Approve)
                        litigationRequestMircoDto.IsDone = "extendLitigationRequest.accepted";
                    else if (extendPeriodRequest.LitigationRequestExtendPeriodRequestStatusId == LitigationRequestExtendPeriodRequestStatus.Reject)
                        litigationRequestMircoDto.IsDone = "extendLitigationRequest.rejected";

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
