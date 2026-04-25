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
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.ModuleServices
{
    public partial class ArbitrationService
    {
        #region CorrectionRequest

        public async Task<BaseResponse> AddArbitrationCorrectionRequest(
            LitigationRequestArbitrationCorrectionRequestCreateDto requestPayload,
            CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                    .Include(x => x.ArbitratorCitizen)
                    .Include(x => x.LitigationRequestArbitratorLists)
                    .Include(x => x.LitigationRequestArbitrationCorrectionRequests)
                    .Include(x => x.ApplicantCitizen)
                    .Include(x => x.DefendantCitizen)
                    .Include(x => x.DefendantFacility)
                    .Include(x => x.Contract.FirstPartyCitizen)
                    .Include(x => x.Contract.FirstPartyFacility)
                    .Include(x => x.Contract.SecondPartyCitizen)
                    .Include(x => x.Contract.SecondPartyFacility)
                    .Where(x => x.Id == requestPayload.LitigationRequestId)
                    .FirstOrDefault();

                if (litigationRequest == null)
                    return new BaseResponse { IsDone = false, Message = "طلب غير موجود" };

                // Check judgment was issued
                if (litigationRequest.LitigationRequestSubStage != LitigationRequestSubStage.JudgmentDecisionIssued)
                    return new BaseResponse { IsDone = false, Message = "لا يمكن تقديم طلب تصحيح قبل إصدار حكم التحكيم" };

                if (!litigationRequest.JudgmentDecisionIssuingDateTime.HasValue)
                    return new BaseResponse { IsDone = false, Message = "لا يمكن تقديم طلب تصحيح، تاريخ إصدار الحكم غير مسجل" };

                // Check 15-day period
                if ((DateTime.Now - litigationRequest.JudgmentDecisionIssuingDateTime.Value).TotalDays > 15)
                    return new BaseResponse { IsDone = false, Message = "لا يمكن تقديم طلب التصحيح، لقد انتهت مدة الـ 15 يومًا المتاحة من تاريخ إصدار الحكم" };

                // Identify party
                LitigationRequestParty requestParty = LitigationRequestParty.None;

                if (litigationRequest.ApplicantCitizenId == referenceId)
                    requestParty = LitigationRequestParty.Plaintiff;
                else if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                    requestParty = LitigationRequestParty.Plaintiff;
                else if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                    requestParty = LitigationRequestParty.Plaintiff;
                else if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                    requestParty = LitigationRequestParty.Defendant;
                else if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                    requestParty = LitigationRequestParty.Defendant;

                if (requestParty == LitigationRequestParty.None)
                    return new BaseResponse { IsDone = false, Message = "غير مصرح لك بتقديم طلب التصحيح" };

                // Check no active (pending) correction request from this party
                bool hasActiveCorrectionRequest = litigationRequest.LitigationRequestArbitrationCorrectionRequests
                    .Any(r => r.RequestParty == requestParty && r.ApproveRejectStatus == null);

                if (hasActiveCorrectionRequest)
                    return new BaseResponse { IsDone = false, Message = "يوجد طلب تصحيح نشط بالفعل على هذا الحكم لنفس الطرف" };

                var correctionRequest = new LitigationRequestArbitrationCorrectionRequest
                {
                    Id = Guid.NewGuid(),
                    LitigationRequestId = requestPayload.LitigationRequestId,
                    RequestCitizenId = referenceId,
                    CorrectionDescription = requestPayload.CorrectionDescription,
                    RequestOn = DateTime.Now,
                    RequestParty = requestParty,
                    CreatedOn = DateTime.Now,
                    CreatedBy = referenceId
                };

                // Attach files
                if (requestPayload.FileIds?.Count > 0)
                {
                    foreach (var fileId in requestPayload.FileIds)
                    {
                        correctionRequest.LitigationRequestArbitrationCorrectionRequestFiles.Add(
                            new LitigationRequestArbitrationCorrectionRequestFile
                            {
                                Id = Guid.NewGuid(),
                                LitigationRequestArbitrationCorrectionRequestId = correctionRequest.Id,
                                CorrectionRequestFileId = fileId,
                                CreatedOn = DateTime.Now
                            });
                    }
                }

                await _repository.AddAsync<LitigationRequestArbitrationCorrectionRequest, Guid>(correctionRequest);

                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    RequestId = litigationRequest.Id,
                    ShowForApplicant = true,
                    LogMessage = $"تم تقديم طلب تصحيح حكم التحكيم من قبل {(requestParty == LitigationRequestParty.Plaintiff ? "المدعى عليه" : " المدعي")}"
                });

                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send notifications
                try
                {
                    var requester = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(c => c.Id == referenceId);
                    var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);

                    var msg = $"تم تقديم طلب تصحيح حكم التحكيم للدعوى رقم {litigationRequest.RequestCode}";
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    string body = string.Empty;
                    if (File.Exists(pathToFile))
                    {
                        using StreamReader sr = File.OpenText(pathToFile);
                        body = await sr.ReadToEndAsync();
                        body = body?.Replace("{content}", msg);
                        body = body?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                    }

                    var notifications = new List<SysNotificationDto>();
                    var emails = new List<string>();

                    // Notify arbitrator
                    if (litigationRequest.ArbitratorCitizenId.HasValue)
                    {
                        notifications.Add(new SysNotificationDto
                        {
                            ActorId = litigationRequest.ArbitratorCitizenId.Value,
                            RequestId = litigationRequest.Id,
                            CreatedOn = DateTime.Now,
                            RequestTypeId = RequestType.LitigationRequest,
                            ActorTypeId = NotificationActorType.Plaintiff,
                            Message = msg
                        });
                        if (!string.IsNullOrEmpty(litigationRequest.ArbitratorCitizen?.Email))
                            emails.Add(litigationRequest.ArbitratorCitizen.Email);
                    }

                    // Notify arbitration admin
                    if (arbitrationAdmin.IsDone && arbitrationAdmin.Data != null)
                    {
                        notifications.Add(new SysNotificationDto
                        {
                            ActorId = (Guid)arbitrationAdmin.Data.Id,
                            RequestId = litigationRequest.Id,
                            CreatedOn = DateTime.Now,
                            RequestTypeId = RequestType.LitigationRequest,
                            ActorTypeId = NotificationActorType.Employee,
                            Message = msg
                        });
                        if (!string.IsNullOrEmpty(arbitrationAdmin.Data.Email))
                            emails.Add(arbitrationAdmin.Data.Email);
                    }

                    if (notifications.Any())
                        await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto
                        {
                            To = emails,
                            Subject = "طلب تصحيح حكم تحكيم",
                            Body = body
                        }, null, notifications);
                }
                catch { }

                //return new BaseResponse { IsDone = true, Message = "تم تقديم طلب التصحيح بنجاح" };
                return await GetArbitrationCorrectionRequestById(requestPayload.LitigationRequestId, new CancellationToken(false));
            }
            catch (Exception ex)
            {
                return new BaseResponse { IsDone = false, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> GetArbitrationCorrectionRequestsForCitizen(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var query = (await _repository.GetQueryable<LitigationRequestArbitrationCorrectionRequest, Guid>())
                .Include(x => x.LitigationRequest).ThenInclude(x => x.Contract.LitigationRequestContractType)
                .Include(x => x.LitigationRequest).ThenInclude(x => x.Contract.FirstPartyCitizen)
                .Include(x => x.LitigationRequest).ThenInclude(x => x.Contract.FirstPartyFacility)
                .Include(x => x.LitigationRequest).ThenInclude(x => x.Contract.SecondPartyCitizen)
                .Include(x => x.LitigationRequest).ThenInclude(x => x.Contract.SecondPartyFacility)
                .Include(x => x.LitigationRequest).ThenInclude(x => x.DefendantCitizen)
                .Include(x => x.LitigationRequest).ThenInclude(x => x.DefendantFacility)
                .Include(x => x.LitigationRequest).ThenInclude(x => x.InternalStatus)
                .Include(x => x.LitigationRequestArbitrationCorrectionRequestFiles)
                    .ThenInclude(f => f.CorrectionRequestFile)
                .Where(x => x.RequestCitizenId == referenceId)
                .OrderByDescending(x => x.RequestOn)
                .AsNoTracking()
                .GetPaged(pageIndex, pageSize);

            var dtos = query?.Results?.Select(r => MapToCorrectionRequestDto(r)).ToList() ?? new();

            return new DataResponse<PagedResult<LitigationRequestArbitrationCorrectionRequestGetDto>>
            {
                Data = new PagedResult<LitigationRequestArbitrationCorrectionRequestGetDto>
                {
                    Results = dtos,
                    CurrentPage = query!.CurrentPage,
                    PageCount = query.PageCount,
                    PageSize = query.PageSize,
                    TotalCount = query.TotalCount
                },
                IsDone = true
            };
        }

        public async Task<BaseResponse> GetArbitrationCorrectionRequestById(Guid litigationRequestId, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.DefendantCitizen)
                .Include(x => x.DefendantFacility)
                .Include(x => x.ArbitratorCitizen!).ThenInclude(x => x.SigntureFile)
                .Include(x => x.ArbitratorCitizen!).ThenInclude(x => x.City)
                .Include(x => x.Area)
                .Include(x => x.City)
                .Include(x => x.LitigationRequestSessions)
                .Include(x => x.ApplicantCitizen)
                .Include(x => x.InternalStatus)
                .Include(x => x.ExternalStatus)
                .Include(x => x.LitigationRequestAmount)
                .Include(x => x.LitigationRequestCategory)
                .Include(x => x.SysExemptionReductionCategory)
                .Include(x => x.LitigationRequestOwnerType)
                .Include(x => x.Contract).ThenInclude(x => x.ContractFile)
                .Include(x => x.Contract).ThenInclude(x => x.AssociationRegistrationCertificateFile)
                .Include(x => x.Contract).ThenInclude(x => x.FoundingLetterFile)
                .Include(x => x.Contract).ThenInclude(x => x.InternalRegulationsFile)
                .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility)
                .Include(x => x.Contract).ThenInclude(x => x.FirstPartyCitizen)
                .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility)
                .Include(x => x.Contract).ThenInclude(x => x.SecondPartyCitizen)
                .Include(x => x.Contract).ThenInclude(x => x.LitigationRequestContractType)
                .Include(x => x.LitigationRequestInvoice)
                .Include(x => x.LitigationRequestArbitratorLists)
                .Include(x => x.LitigationRequestArbitrationCorrectionRequests)
                    .ThenInclude(c => c.RequestCitizen)
                .Include(x => x.LitigationRequestArbitrationCorrectionRequests)
                    .ThenInclude(c => c.LitigationRequestArbitrationCorrectionRequestFiles)
                    .ThenInclude(f => f.CorrectionRequestFile)
                .AsNoTracking()
                .Where(x => x.Id == litigationRequestId)
                .FirstOrDefault();

            if (litigationRequest == null)
                return new BaseResponse { IsDone = false, Message = "الطلب غير موجود" };

            var records = litigationRequest.LitigationRequestArbitrationCorrectionRequests?
                .OrderByDescending(x => x.RequestOn).ToList() ?? new();

            var latestRecord = records.FirstOrDefault();

            // Access control: requester, main arbitrator, or panel member
            bool isArbitrator = litigationRequest.ArbitratorCitizenId == referenceId
                || litigationRequest.LitigationRequestArbitratorLists.Any(a => a.ArbitratorCitizenId == referenceId);

            //if (!isParty && !isArbitrator)
            //    return new BaseResponse { IsDone = false, Message = "غير مصرح بالوصول" };

            // Map litigation request to short DTO (same as GetLitigationRequestForReport)
            string base64String = string.Empty;
            var requestDto = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            if (requestDto.LitigationRequestAmount != null)
            {
                requestDto.LitigationRequestAmount.Fees = litigationRequest.LitigationRequestInvoice?.TotalAmount ?? 0;
            }
            else
            {
                var minRequestAmount = (await _repository.GetQueryable<SysLitigationRequestAmount, Guid>())
                    .Include(x => x.LitigationRequestFees)
                    .Where(x => x.FromAmount == 0).FirstOrDefault();

                requestDto.LitigationRequestAmount = new SysLitigationRequestAmountCreateUpdateDto
                {
                    Fees = minRequestAmount?.LitigationRequestFees?.FirstOrDefault()?.ArbitrationFees ?? 0,
                };
            }

            if (!string.IsNullOrEmpty(litigationRequest.ArbitratorCitizen?.SigntureFile?.FilePath))
            {
                string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), litigationRequest.ArbitratorCitizen.SigntureFile.FilePath);
                if (File.Exists(inputFilePath))
                {
                    byte[] fileBytes = File.ReadAllBytes(inputFilePath);
                    base64String = Convert.ToBase64String(fileBytes);
                }
            }
            if (requestDto != null && !string.IsNullOrEmpty(base64String))
                requestDto.ArbitratorCitizen.SigntureFile = base64String;

            // AllowAdd logic: only allow new request if no active or pending requests (as per user feedback)
            bool hasActiveOrPending = records.Any(r => r.ApproveRejectStatus == null);
            bool allowAdd = !hasActiveOrPending && !isArbitrator;

            // AllowResponse: arbitrator can respond only if latest request is still pending
            bool allowResponse = isArbitrator && latestRecord?.ApproveRejectStatus == null;

            return new DataResponse<LitigationRequestForArbitrationCorrectionRequestDto>
            {
                Data = new LitigationRequestForArbitrationCorrectionRequestDto
                {
                    LitigationRequestShortDto = requestDto,
                    AllowAdd = allowAdd,
                    AllowResponse = allowResponse,
                    CorrectionRequests = records.Select(r => MapToCorrectionRequestDto(r)).ToList()
                },
                IsDone = true
            };
        }

        public async Task<BaseResponse> AddArbitrationCorrectionRequestResponse(
            LitigationRequestArbitrationCorrectionRequestResponseDto requestPayload,
            CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                if (requestPayload.Response == ApproveRejectStatus.Reject && string.IsNullOrWhiteSpace(requestPayload.RejectReason))
                    return new BaseResponse { IsDone = false, Message = "يجب إدخال سبب الرفض" };

                if (requestPayload.Response == ApproveRejectStatus.Approve && string.IsNullOrWhiteSpace(requestPayload.CorrectionDecisionText))
                    return new BaseResponse { IsDone = false, Message = "يجب إدخال نص قرار التصحيح" };

                var correctionRequest = (await _repository.GetQueryable<LitigationRequestArbitrationCorrectionRequest, Guid>())
                    .Include(x => x.LitigationRequest).ThenInclude(x => x.ArbitratorCitizen)
                    .Include(x => x.LitigationRequest).ThenInclude(x => x.LitigationRequestArbitratorLists)
                    .Include(x => x.LitigationRequest).ThenInclude(x => x.Contract.FirstPartyCitizen)
                    .Include(x => x.LitigationRequest).ThenInclude(x => x.Contract.FirstPartyFacility)
                    .Include(x => x.LitigationRequest).ThenInclude(x => x.Contract.SecondPartyCitizen)
                    .Include(x => x.LitigationRequest).ThenInclude(x => x.Contract.SecondPartyFacility)
                    .Include(x => x.LitigationRequest).ThenInclude(x => x.ApplicantCitizen)
                    .Include(x => x.LitigationRequest).ThenInclude(x => x.DefendantCitizen)
                    .Include(x => x.LitigationRequest).ThenInclude(x => x.LitigationRequestArbitrationReports)
                    .FirstOrDefault(x => x.Id == requestPayload.Id && x.LitigationRequestId == requestPayload.LitigationRequestId);

                if (correctionRequest == null)
                    return new BaseResponse { IsDone = false, Message = "الطلب غير موجود" };

                var litigationRequest = correctionRequest.LitigationRequest;

                bool isArbitrator = litigationRequest.ArbitratorCitizenId == referenceId
                    || litigationRequest.LitigationRequestArbitratorLists.Any(a => a.ArbitratorCitizenId == referenceId);

                if (!isArbitrator)
                    return new BaseResponse { IsDone = false, Message = "غير مصرح، هذه الخدمة متاحة للمحكم فقط" };

                if (correctionRequest.ApproveRejectStatus != null)
                    return new BaseResponse { IsDone = false, Message = "تم البت في هذا الطلب مسبقًا" };

                correctionRequest.ApproveRejectStatus = requestPayload.Response;
                correctionRequest.ResponseOn = DateTime.Now;

                if (requestPayload.Response == ApproveRejectStatus.Reject)
                    correctionRequest.RejectReason = requestPayload.RejectReason;
                else
                {
                    correctionRequest.CorrectionDecisionText = requestPayload.CorrectionDecisionText;
                    var report = litigationRequest.LitigationRequestArbitrationReports?.LastOrDefault();
                    if (report != null)
                    {
                        report.JudgmentDecisionText = requestPayload.CorrectionDecisionText;
                        _repository.Update<LitigationRequestArbitrationReport, Guid>(report);
                    }
                }

                _repository.Update<LitigationRequestArbitrationCorrectionRequest, Guid>(correctionRequest);

                string decisionAr = requestPayload.Response == ApproveRejectStatus.Approve ? "قبول" : "رفض";
                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    RequestId = litigationRequest.Id,
                    ShowForApplicant = true,
                    LogMessage = $"تم اتخاذ قرار بشأن طلب تصحيح الحكم: {decisionAr}"
                });

                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send notifications
                try
                {
                    var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                    var msg = $"تم {decisionAr} طلب تصحيح حكم التحكيم للدعوى رقم {litigationRequest.RequestCode}";
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    string body = string.Empty;
                    if (File.Exists(pathToFile))
                    {
                        using StreamReader sr = File.OpenText(pathToFile);
                        body = await sr.ReadToEndAsync();
                        body = body?.Replace("{content}", msg);
                        body = body?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                    }

                    var notifications = new List<SysNotificationDto>();
                    var emails = new List<string>();

                    // Notify all parties
                    var partyIds = new List<(Guid? id, string? email)>
                    {
                        (litigationRequest.ApplicantCitizenId, litigationRequest.ApplicantCitizen?.Email),
                        (litigationRequest.Contract.FirstPartyCitizenId, litigationRequest.Contract.FirstPartyCitizen?.Email),
                        (litigationRequest.Contract.SecondPartyCitizenId, litigationRequest.Contract.SecondPartyCitizen?.Email),
                    };

                    foreach (var (id, email) in partyIds)
                    {
                        if (id.HasValue && id.Value != Guid.Empty)
                        {
                            notifications.Add(new SysNotificationDto
                            {
                                ActorId = id.Value,
                                RequestId = litigationRequest.Id,
                                CreatedOn = DateTime.Now,
                                RequestTypeId = RequestType.LitigationRequest,
                                ActorTypeId = NotificationActorType.Plaintiff,
                                Message = msg
                            });
                            if (!string.IsNullOrEmpty(email))
                                emails.Add(email);
                        }
                    }

                    // Notify arbitration admin
                    if (arbitrationAdmin.IsDone && arbitrationAdmin.Data != null)
                    {
                        notifications.Add(new SysNotificationDto
                        {
                            ActorId = (Guid)arbitrationAdmin.Data.Id,
                            RequestId = litigationRequest.Id,
                            CreatedOn = DateTime.Now,
                            RequestTypeId = RequestType.LitigationRequest,
                            ActorTypeId = NotificationActorType.Employee,
                            Message = msg
                        });
                        if (!string.IsNullOrEmpty(arbitrationAdmin.Data.Email))
                            emails.Add(arbitrationAdmin.Data.Email);
                    }

                    if (notifications.Any())
                        await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto
                        {
                            To = emails,
                            Subject = "قرار طلب تصحيح حكم التحكيم",
                            Body = body
                        }, null, notifications);
                }
                catch { }

                return await GetArbitrationCorrectionRequestById(requestPayload.LitigationRequestId, new CancellationToken());
            }
            catch (Exception ex)
            {
                return new BaseResponse { IsDone = false, Message = ex.Message };
            }
        }

        public async Task<BaseResponse> GetArbitrationCorrectionRequestsForArbitrator(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.InternalStatus)
                .Include(x => x.ArbitratorCitizen!).ThenInclude(x => x.City)
                .Include(x => x.LitigationRequestArbitratorLists)
                .Include(x => x.Contract).ThenInclude(x => x.LitigationRequestContractType)
                .Include(x => x.Contract).ThenInclude(x => x.FirstPartyCitizen)
                .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility!).ThenInclude(x => x.Citizen)
                .Include(x => x.Contract).ThenInclude(x => x.SecondPartyCitizen)
                .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility!).ThenInclude(x => x.Citizen)
                .Include(x => x.DefendantCitizen)
                .Include(x => x.DefendantFacility)
                .Include(x => x.LitigationRequestArbitrationCorrectionRequests)
                    .ThenInclude(c => c.RequestCitizen)
                .Include(x => x.LitigationRequestArbitrationCorrectionRequests)
                    .ThenInclude(c => c.LitigationRequestArbitrationCorrectionRequestFiles)
                    .ThenInclude(f => f.CorrectionRequestFile)
                .Where(x => (x.ArbitratorCitizenId == referenceId
                    || x.LitigationRequestArbitratorLists.Any(a => a.ArbitratorCitizenId == referenceId))
                    && x.LitigationRequestArbitrationCorrectionRequests.Any())
                .AsNoTracking()
                .ToList();

            var allCorrectionRequests = litigationRequests
                .SelectMany(x => x.LitigationRequestArbitrationCorrectionRequests)
                .ToList()
                .GroupBy(c => c.LitigationRequestId)
                .Select(g => g
                    .OrderByDescending(c => c.CreatedOn)
                    .First()
                )
                .ToList();

            var totalCount = allCorrectionRequests.Count;
            var pagedResults = allCorrectionRequests
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(r => MapToCorrectionRequestDto(r))
                .ToList();

            return new DataResponse<PagedResult<LitigationRequestArbitrationCorrectionRequestGetDto>>
            {
                Data = new PagedResult<LitigationRequestArbitrationCorrectionRequestGetDto>
                {
                    Results = pagedResults,
                    CurrentPage = pageIndex,
                    PageCount = (int)Math.Ceiling((double)totalCount / pageSize),
                    PageSize = pageSize,
                    TotalCount = totalCount
                },
                IsDone = true
            };
        }

        private LitigationRequestArbitrationCorrectionRequestGetDto MapToCorrectionRequestDto(LitigationRequestArbitrationCorrectionRequest r)
        {
            var lit = r.LitigationRequest;
            var arbitrator = lit?.ArbitratorCitizen;
            var firstParty = lit?.Contract?.FirstPartyCitizen ?? lit?.Contract?.FirstPartyFacility?.Citizen;
            var secondParty = lit?.Contract?.SecondPartyCitizen ?? lit?.Contract?.SecondPartyFacility?.Citizen;

            return new LitigationRequestArbitrationCorrectionRequestGetDto
            {
                Id = r.Id,
                LitigationRequestId = r.LitigationRequestId,
                RequestCitizenId = r.RequestCitizenId,
                RequestCitizenName = r.RequestCitizen?.FullName,
                CorrectionDescription = r.CorrectionDescription,
                RequestDate = r.RequestOn,
                RequestParty = r.RequestParty,
                ApproveRejectStatus = r.ApproveRejectStatus,
                RejectReason = r.RejectReason,
                CorrectionDecisionText = r.CorrectionDecisionText,
                ResponseOn = r.ResponseOn,
                Files = r.LitigationRequestArbitrationCorrectionRequestFiles?.Select(f => new LitigationRequestArbitrationCorrectionRequestFileGetDto
                {
                    Id = f.Id,
                    CorrectionRequestFileId = f.CorrectionRequestFileId,
                    FilePath = f.CorrectionRequestFile?.FilePath,
                    FileType = f.CorrectionRequestFile?.FileType
                }).ToList() ?? new(),

                // رقم الدعوى - تاريخ إصدار الحكم - حالة الدعوى
                RequestCode = lit?.RequestCode,
                JudgmentDecisionDate = lit?.JudgmentDecisionIssuingDateTime,
                Status = lit?.InternalStatus?.RequestStatus,

                // هيئة التحكيم - المحكم العقاري
                ArbitratorName = arbitrator?.FullName,
                ArbitratorIdentityNumber = arbitrator?.IdentityNumber,
                ArbitratorMobile = arbitrator?.Mobile,
                ArbitratorCity = arbitrator?.City?.CityName,
                ArbitratorMembershipCode = arbitrator?.Code,

                // الطرف الأول - المحتكم
                FirstPartyName = firstParty?.FullName,
                FirstPartyIdentityNumber = firstParty?.IdentityNumber,
                FirstPartyRole = "المحتكم",
                FirstPartyAddress = firstParty?.NationalAddress,
                FirstPartyMobile = firstParty?.Mobile,
                FirstPartyEmail = firstParty?.Email,

                // الطرف الثاني - المحتكم ضده
                SecondPartyName = secondParty?.FullName,
                SecondPartyIdentityNumber = secondParty?.IdentityNumber,
                SecondPartyRole = "المحتكم ضده",
                SecondPartyAddress = secondParty?.NationalAddress,
                SecondPartyMobile = secondParty?.Mobile,
                SecondPartyEmail = secondParty?.Email
            };
        }

        #endregion
    }
}
