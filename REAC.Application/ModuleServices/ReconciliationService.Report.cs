using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.ModuleServices
{
    public partial class ReconciliationService
    {
        #region Report
        public async Task<BaseResponse> AddReconciliationReport(WritingMinutesOfTheMeetingRequestPayload requestPayload, CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestReconciliationReports)
                        .Include(x => x.ArbitratorCitizen)
                        .Include(x => x.ReformerCitizen)
                        .Include(x => x.ApplicantCitizen)
                        .Include(x => x.Contract)
                        .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                        .Include(x => x.Contract.FirstPartyCitizen)
                        .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                        .Include(x => x.Contract.SecondPartyCitizen)
                        .Where(x => x.Id == requestPayload.RequestId & x.ReformerCitizenId == referenceId)
                        .Single();

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                bool allowAdd = true;
                var msg = string.Empty;
                if (litigationRequest.LitigationRequestReconciliationReports.Count() > 0)
                    allowAdd = false;

                // Reconcilition is done => old flow 
                // Reconcilition is  not done => check with category and move to arbitiration 
                // giving up or trady platform => notify and edit the request
                switch (requestPayload.ReconciliationRequestDecisionStatus)
                {
                    case ReconciliationRequestDecision.ReconciliationWasMade:
                        //litigationRequest.InternalStatusId = RequestStatus.ReconciliationWasMade;
                        //litigationRequest.ExternalStatusId = RequestStatus.ReconciliationWasMade;
                        //litigationRequest.IsActive = false;
                        litigationRequest.InternalStatusId = RequestStatus.TheMinutesOfTheMeetingWereWritten;
                        litigationRequest.ExternalStatusId = RequestStatus.TheMinutesOfTheMeetingWereWritten;
                        litigationRequest.IsMinutesOfMeetingApprovalRequired = true;
                        litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestReformerDecisionAdded;
                        // Send Notification
                        // تم الصلح
                        // تم إضافة محضر الصلح
                        try
                        {
                            //msg = (await _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Report_Creation)).Data?.ToString();
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Request_Added).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            await SendNotificationMSWithAllTypes(msg, litigationRequest, "Reconciliation Request Added");
                        }
                        catch (Exception e)
                        { }
                        break;
                    case ReconciliationRequestDecision.WaiverOfRight:
                        litigationRequest.InternalStatusId = RequestStatus.WaiverOfRight;
                        litigationRequest.ExternalStatusId = RequestStatus.TheMinutesOfTheMeetingWereWritten;
                        litigationRequest.IsMinutesOfMeetingApprovalRequired = true;
                        litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestReformerDecisionAdded;
                        // Send Notification
                        // تم التنازل عن الحق
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_WaiverOfRight).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            await SendNotificationMSWithAllTypes(msg, litigationRequest, "Reconciliation Litigation Request For defientive Report Reply Added");
                        }
                        catch (Exception e)
                        { }
                        break;

                    case ReconciliationRequestDecision.WaiverOfTheLitigation:
                        litigationRequest.InternalStatusId = RequestStatus.WaiverOfTheLitigation;
                        litigationRequest.ExternalStatusId = RequestStatus.TheMinutesOfTheMeetingWereWritten;
                        litigationRequest.IsMinutesOfMeetingApprovalRequired = true;
                        litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestReformerDecisionAdded;
                        //litigationRequest.IsActive = false;
                        //litigationRequest.IsFinished = true;
                        // Send Notification
                        // تم التنازل عن الدعوى
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_WaiverOfTheLitigation).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            await SendNotificationMSWithAllTypes(msg, litigationRequest, "Reconciliation Litigation Request For defientive Report Reply Added");
                        }
                        catch (Exception e)
                        { }
                        break;
                    case ReconciliationRequestDecision.ReconciliationOnTradyPlatform:
                        //if (litigationRequest.LitigationRequestCategoryId == LitigationRequestCategory.ReconciliationAndArbitration)
                        //{
                        litigationRequest.InternalStatusId = RequestStatus.ReconciliationOnTradyPlatform;
                        litigationRequest.ExternalStatusId = RequestStatus.ReconciliationOnTradyPlatform;
                        litigationRequest.IsActive = false;
                        litigationRequest.IsFinished = true;
                        litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestReformerDecisionAdded;
                        // Send Notification
                        //  الصلح بواسطة منصة تراضى 
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ReconciliationOnTradyPlatform).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            await SendNotificationMSWithAllTypes(msg, litigationRequest, "Reconciliation Litigation Request For Report Reply Added");
                        }
                        catch (Exception e)
                        {


                        }
                        //}
                        break;
                    case ReconciliationRequestDecision.ReconciliationWasNotMade:
                        if (litigationRequest.LitigationRequestCategoryId == LitigationRequestCategory.ReconciliationAndArbitration)
                        {
                            litigationRequest.InternalStatusId = RequestStatus.ReconciliationWasNotMadeAndMoveToArbitration;
                            litigationRequest.ExternalStatusId = RequestStatus.ReconciliationWasNotMadeAndMoveToArbitration;
                            litigationRequest.LitigationRequestStage = LitigationRequestStage.Arbitration;
                            litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.ArbitratorSelectionPeriod;
                            // Send Notification
                            //عدم الصلح  
                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_ReconciliationWasNotMadeAndMoveToArbitration).Result.Data?.Value;
                                msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                await SendNotificationMSWithAllTypes(msg, litigationRequest, "Reconciliation Litigation Request For Report Reply Added");
                            }
                            catch (Exception e)
                            {


                            }
                        }
                        else
                        {
                            litigationRequest.InternalStatusId = RequestStatus.ReconciliationWasNotMade;
                            litigationRequest.ExternalStatusId = RequestStatus.ReconciliationWasNotMade;
                            litigationRequest.IsActive = false;
                            litigationRequest.IsFinished = true;

                            // Send Notification
                            // عدم الصلح

                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_ReconciliationWasNotMade).Result.Data?.Value;
                                msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                await SendNotificationMSWithAllTypes(msg, litigationRequest, "Reconciliation Litigation Request For Report Reply Added");
                            }
                            catch (Exception e)
                            { }
                        }
                        break;
                    default:
                        break;
                }
                if (allowAdd)
                {
                    LitigationRequestReconciliationReport litigationRequestReconciliationReport = new LitigationRequestReconciliationReport()
                    {
                        Id = Guid.NewGuid(),
                        ReportDate = DateTime.Now,
                        ReconciliationSessionDetails = requestPayload.ReconciliationSessionDetails,
                        ModificationRequest = requestPayload.ModificationRequest,
                        LitigationRequestId = requestPayload.RequestId,
                    };

                    if (requestPayload.LitigationRequestMeetingReconciliationItems != null)
                    {
                        requestPayload.LitigationRequestMeetingReconciliationItems.ToList().ForEach(t =>
                        {
                            litigationRequestReconciliationReport.LitigationRequestMeetingReconciliationItems.Add(new LitigationRequestReconciliationReportItem()
                            {
                                Id = Guid.NewGuid(),
                                LitigationRequestReconciliationReportId = litigationRequestReconciliationReport.Id,
                                ReconciliationItem = t.ReconciliationItem
                            });


                        });
                    }
                    

                    await _repository.AddAsync<LitigationRequestReconciliationReport, Guid>(litigationRequestReconciliationReport);

                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationRequest.Id,
                        ShowForApplicant = false,
                        LogMessage = "تم إضافة محضر الصلح"
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
                        Message = "لا يمكن إضافة محضر جديد"
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
            return await GetLitigationRequestForReport(requestPayload.RequestId, cancellationToken);
        }
        public async Task<BaseResponse> GetLitigationRequestForReport(Guid requestId, CancellationToken cancellationToken)
        {

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            if (referenceId == Guid.Empty)
                return new BaseResponse() { IsDone = false, Message = "غير مصرح" };

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.ReformerCitizen)
                         .ThenInclude(x=>x.SigntureFile)
                         .Include(x => x.Area)
                         .Include(x => x.City)
                         .Include(x => x.LitigationRequestSessions)
                         .Include(x => x.LitigationRequestReconciliationReports)
                         .ThenInclude(x => x.LitigationRequestMeetingReconciliationItems)
                .Include(x => x.LitigationRequestReformerAssigningRequests)
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
                         .ThenInclude(x => x.SigntureFile)
                         .Include(x => x.Contract.SecondPartyFacility)
                         .Include(x => x.Contract.SecondPartyCitizen)
                         .ThenInclude(x => x.SigntureFile)
                         .Include(x => x.Contract.LitigationRequestContractType)
                         .AsNoTracking()
                         .FirstOrDefault(x => x.Id == requestId
                         &&
                       x.LitigationRequestReformerAssigningRequests.Any(l => l.CitizenId == referenceId) & x.Id == requestId
                         );
            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "غير مصرح" };
            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);
            string base64ReformerSignture = string.Empty;
            if (!string.IsNullOrEmpty(litigationRequest?.ReformerCitizen?.SigntureFile?.FilePath))
            {
                string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), litigationRequest?.ReformerCitizen?.SigntureFile?.FilePath);
                // Read file and convert to Base64
                byte[] fileBytes = File.ReadAllBytes(inputFilePath);
                base64ReformerSignture = Convert.ToBase64String(fileBytes);
                request.ReformerCitizen.SigntureFile = base64ReformerSignture;
            }

            string base64FirstPartySignture = string.Empty;
            if (!string.IsNullOrEmpty(litigationRequest?.Contract?.FirstPartyCitizen?.SigntureFile?.FilePath))
            {
                string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), litigationRequest?.Contract?.FirstPartyCitizen?.SigntureFile?.FilePath);
                // Read file and convert to Base64
                byte[] fileBytes = File.ReadAllBytes(inputFilePath);
                base64FirstPartySignture = Convert.ToBase64String(fileBytes);
                request.ReformerCitizen.SigntureFile = base64FirstPartySignture;
            }
            string base64SecondPartySignture = string.Empty;
            if (!string.IsNullOrEmpty(litigationRequest?.Contract?.SecondPartyCitizen?.SigntureFile?.FilePath))
            {
                string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), litigationRequest?.Contract?.SecondPartyCitizen?.SigntureFile?.FilePath);
                // Read file and convert to Base64
                byte[] fileBytes = File.ReadAllBytes(inputFilePath);
                base64SecondPartySignture = Convert.ToBase64String(fileBytes);
                request.ReformerCitizen.SigntureFile = base64SecondPartySignture;
            }

            var reports = _mapper.Map<List<LitigationRequestReconciliationReportCreateUpdateDto>>(litigationRequest?.LitigationRequestReconciliationReports);

            bool isDone = false;
            bool allowAdd = true;
            bool allowDecision = true;


            if (litigationRequest.IsFinished.HasValue)
            {
                if (litigationRequest.IsFinished.Value)
                {
                    allowAdd = false;
                    allowDecision = false;
                    isDone = true;
                }
            }
            if (!isDone)
            {
                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Reconciliation)
                {
                    allowAdd = false;
                    allowDecision = false;
                }
                else
                {
                    if (litigationRequest?.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestPartiesDecisionAdded)
                    {
                        allowAdd = false;
                    }
                    else
                    {
                        if (litigationRequest?.LitigationRequestReconciliationReports != null)
                        {
                            if (litigationRequest.LitigationRequestReconciliationReports.Count > 0)
                                allowAdd = false;
                        }
                    }
                }
            }
            if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestPartiesDecisionAdded)
            {
                allowDecision = false;
            }

            try
            {
                int LitigationRequestReconciliationReportDecisionPeriod = int.Parse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestReconciliationReportDecisionPeriod).Result.Data?.Value);
                double totalHours = 0;
                if (litigationRequest.LitigationRequestReconciliationReports is not null & litigationRequest.LitigationRequestReconciliationReports.Count() > 0)
                    totalHours += (DateTime.Now - litigationRequest.LitigationRequestReconciliationReports.FirstOrDefault().ReportDate).TotalHours;
                if (totalHours >= LitigationRequestReconciliationReportDecisionPeriod)
                {
                    allowDecision = true;
                }
            }
            catch { }

            return new DataResponse<LitigationRequestForReportDto>()
            {
                Data = new LitigationRequestForReportDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    AllowDecision = allowDecision,
                    Report = reports.FirstOrDefault()
                },
                IsDone = true,
            };

        }
        public async Task<BaseResponse> GetLitigationRequestForReport(CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                                      .Include(x => x.LitigationRequestSessions)
                                      .Include(x => x.LitigationRequestReconciliationReports)
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
                                      & x.LitigationRequestSessions.Where(t => t.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation).Count() > 0
                                      & x.RequestCode != null
                                      & x.InternalStatusId != RequestStatus.Rejected
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration).
                                      OrderByDescending(x => x.CreatedOn).ToList();

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

                            if (litigationRequest?.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestPartiesDecisionAdded)
                                allowAdd = false;
                            else
                            {
                                if (litigationRequest?.LitigationRequestReconciliationReports != null)
                                {
                                    if (litigationRequest?.LitigationRequestReconciliationReports.Count > 0)
                                    {
                                        allowAdd = false;
                                    }
                                }
                            }
                        }
                    }
                    try
                    {
                        int LitigationRequestReconciliationReportDecisionPeriod = int.Parse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestReconciliationReportDecisionPeriod).Result.Data?.Value);
                        double totalHours = 0;
                        if(litigationRequest.LitigationRequestReconciliationReports is not null & litigationRequest.LitigationRequestReconciliationReports.Count() > 0)
                        totalHours +=  (DateTime.Now - litigationRequest.LitigationRequestReconciliationReports.FirstOrDefault().ReportDate).TotalHours;
                        if (totalHours >= LitigationRequestReconciliationReportDecisionPeriod)
                        {
                            allowAdd = true;
                        }
                    }
                    catch { }

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
        #endregion


        #region Nafath
        public async Task<BaseResponse> CallNafathForReconciliationReport(Guid requestId, ApproveRejectStatus agreementRequestStatus)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                              .AsNoTracking()
                              .FirstOrDefault(x => x.Id == requestId);
                var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                              .AsNoTracking()
                              .FirstOrDefault(x => x.Id == referenceId);
                if (litigationRequest is not null & citizen is not null)
                {
                    var nafathReconciliationReportResponse = await _nafathService.NafathReconciliationReport(
                         new NafathRequestDto() { IsRegister = false, NationalId = citizen.IdentityNumber },
                         new NafathReconciliationReportTransactionDto()
                         {
                             LitigationRequestId = litigationRequest.Id,
                             NationalId = citizen.IdentityNumber,
                             RequestMinutesOfMeetingApprovalStatus = agreementRequestStatus
                         });
                    return nafathReconciliationReportResponse;
                }
                return new BaseResponse() { IsDone = false, Message = "خطأ في البيانات المدخله" };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
            }
        }
        #endregion
    }
}
