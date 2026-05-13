using System.Text;
using System.Text.Json;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using REAC.Application.Contracts.IAppServices;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Integration.Invoice;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Invoice;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.ModuleServices
{
    public interface ILitigationRequestAdminService
    {
        Task<BaseResponse> GetClosedLitigationRequests(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestReopenInfo(Guid litigationRequestId, CancellationToken cancellationToken);
        Task<BaseResponse> ReopenLitigationRequest(ReopenLitigationRequestDto requestPayload, CancellationToken cancellationToken);
    }

    public class LitigationRequestAdminService : ILitigationRequestAdminService
    {
        private readonly IEntityRepository _repository;
        private readonly IJwtReader _jwtReader;
        private readonly INotificationService _notificationService;
        private readonly ISysEmployeeService _sysEmployeeService;
        private readonly ISysSettingService _settingService;
        private readonly IGEELoginService _gEELoginService;

        public LitigationRequestAdminService(
            IEntityRepository repository,
            IJwtReader jwtReader,
            INotificationService notificationService,
            ISysEmployeeService sysEmployeeService,
            ISysSettingService settingService,
            IGEELoginService gEELoginService)
        {
            _repository = repository;
            _jwtReader = jwtReader;
            _notificationService = notificationService;
            _sysEmployeeService = sysEmployeeService;
            _settingService = settingService;
            _gEELoginService = gEELoginService;
        }

        /// <summary>
        /// Get list of closed (Cancelled/Rejected) litigation requests for admin
        /// </summary>
        public async Task<BaseResponse> GetClosedLitigationRequests(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            var query = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.InternalStatus)
                .Include(x => x.ApplicantCitizen)
                .Include(x => x.LitigationRequestCategory)
                .Include(x => x.Contract).ThenInclude(x => x.LitigationRequestContractType)
                .Where(x => x.InternalStatusId == RequestStatus.Cancelled
                    || x.InternalStatusId == RequestStatus.Rejected)
                .OrderByDescending(x => x.CreatedOn)
                .AsNoTracking()
                .GetPaged(pageIndex, pageSize);

            var dtos = query?.Results?.Select(r => new LitigationRequestClosedListDto
            {
                LitigationRequestId = r.Id,
                RequestCode = r.RequestCode,
                RequestTypeName = GetCategoryName(r.LitigationRequestCategoryId),
                InternalStatusName = r.InternalStatus?.RequestStatus,
                SubmissionDate = r.SentOn ?? r.CreatedOn,
                CancellationDate = r.CreatedOn,
                ApplicantName = r.ApplicantCitizen?.FullName
            }).ToList() ?? new();

            return new DataResponse<PagedResult<LitigationRequestClosedListDto>>
            {
                Data = new PagedResult<LitigationRequestClosedListDto>
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

        /// <summary>
        /// Get detailed reopen info for a specific litigation request (data dictionary view)
        /// </summary>
        public async Task<BaseResponse> GetLitigationRequestReopenInfo(Guid litigationRequestId, CancellationToken cancellationToken)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x=>x.WorkFlowProcessingLogs)
                .ThenInclude(x=>x.WorkFlowStep)
                .Include(x => x.InternalStatus)
                .Include(x => x.ApplicantCitizen)
                .Include(x => x.LitigationRequestCategory)
                .Include(x => x.Contract).ThenInclude(x => x.LitigationRequestContractType)
                .Include(x => x.LitigationRequestReopenLogs)
                .AsNoTracking()
                .Where(x => x.Id == litigationRequestId)
                .FirstOrDefault();

            if (litigationRequest == null)
                return new BaseResponse { IsDone = false, Message = "الطلب غير موجود" };

            bool isCancelledOrRejected = litigationRequest.InternalStatusId == RequestStatus.Cancelled
                || litigationRequest.InternalStatusId == RequestStatus.Rejected;

            // Determine the target status after reopen
            var (newInternalStatus, newStage, newSubStage) = DetermineReopenTarget(litigationRequest);
            string? newStatusName = await GetStatusName(newInternalStatus);

            // Get invoice payment statuses
            var invoices = (await _repository.GetQueryable<SysInvoice, Guid>())
                .Where(x => x.RequestId == litigationRequestId)
                .AsNoTracking()
                .ToList();

            string adminFeesStatus = GetInvoicePaymentStatus(invoices, InvoiceType.AdministrativeForLawsuitFees);
            string reconciliationFeesStatus = GetInvoicePaymentStatus(invoices, InvoiceType.ReconciliationFees);
            string arbitrationFeesStatus = GetInvoicePaymentStatus(invoices, InvoiceType.ArbitrationFees);

            var lastReopenLog = litigationRequest.LitigationRequestReopenLogs
                .OrderByDescending(x => x.ReopenedOn)
                .FirstOrDefault();

            var dto = new LitigationRequestReopenInfoDto
            {
                LitigationRequestId = litigationRequest.Id,
                RequestCode = litigationRequest.RequestCode,
                RequestTypeName = GetCategoryName(litigationRequest.LitigationRequestCategoryId),
                AdministrativeFeesPaymentStatus = adminFeesStatus,
                ReconciliationFeesPaymentStatus = reconciliationFeesStatus,
                ArbitrationFeesPaymentStatus = arbitrationFeesStatus,
                SubmissionDate = litigationRequest.SentOn ?? litigationRequest.CreatedOn,
                CancellationDate = null,
                StatusBeforeCancellation = litigationRequest.WorkFlowProcessingLogs?.OrderBy(x=>x.CreatedOn).FirstOrDefault()?.WorkFlowStep.StepName,
                StatusAfterReopen = newStatusName,
                ReopenDate = lastReopenLog?.ReopenedOn,
                ReopenReason = lastReopenLog?.ReopenReason,
                AllowReopen = isCancelledOrRejected
            };

            return new DataResponse<LitigationRequestReopenInfoDto>
            {
                Data = dto,
                IsDone = true
            };
        }

        /// <summary>
        /// Reopen a closed litigation request (admin only)
        /// </summary>
        public async Task<BaseResponse> ReopenLitigationRequest(ReopenLitigationRequestDto requestPayload, CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                if (string.IsNullOrWhiteSpace(requestPayload.ReopenReason))
                    return new BaseResponse { IsDone = false, Message = "يجب إدخال سبب إعادة فتح الطلب" };

                if (requestPayload.ReopenReason.Length > 500)
                    return new BaseResponse { IsDone = false, Message = "سبب إعادة الفتح يجب ألا يتجاوز 500 حرف" };

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                    .Include(x => x.InternalStatus)
                    .Include(x => x.ApplicantCitizen)
                    .Include(x => x.Contract).ThenInclude(x => x.FirstPartyCitizen)
                    .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility)
                    .Include(x => x.Contract).ThenInclude(x => x.SecondPartyCitizen)
                    .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility)
                    .Include(x => x.DefendantCitizen)
                    .Include(x => x.ArbitratorCitizen)
                    .Include(x => x.ReformerCitizen)
                    .Include(x => x.WorkFlowProcessingLogs)
                    .Where(x => x.Id == requestPayload.LitigationRequestId)
                    .FirstOrDefault();

                if (litigationRequest == null)
                    return new BaseResponse { IsDone = false, Message = "الطلب غير موجود" };

                // Only allow reopening Cancelled or Rejected requests
                if (litigationRequest.InternalStatusId != RequestStatus.Cancelled
                    && litigationRequest.InternalStatusId != RequestStatus.Rejected)
                    return new BaseResponse { IsDone = false, Message = "لا يمكن إعادة فتح الطلب إلا للطلبات الملغاة أو المرفوضة" };

                // Save previous state
                var previousInternalStatus = litigationRequest.InternalStatusId;
                var previousStage = litigationRequest.LitigationRequestStage;
                var previousSubStage = litigationRequest.LitigationRequestSubStage;

                // Determine the reopen target based on business rules
                var (newInternalStatus, newStage, newSubStage) = DetermineReopenTarget(litigationRequest);

                // Update litigation request status
                litigationRequest.InternalStatusId = newInternalStatus;
                litigationRequest.ExternalStatusId = newInternalStatus;
                litigationRequest.LitigationRequestStage = newStage;
                if (newSubStage.HasValue)
                    litigationRequest.LitigationRequestSubStage = newSubStage.Value;
                litigationRequest.IsActive = true;
                litigationRequest.IsFinished = false;

                _repository.Update<LitigationRequest, Guid>(litigationRequest);

                // Update last WorkFlowProcessingLog to IsFinished = false
                var lastProcessingLog = litigationRequest.WorkFlowProcessingLogs
                    .OrderBy(x => x.CreatedOn)
                    .LastOrDefault();
                if (lastProcessingLog != null)
                {
                    lastProcessingLog.IsFinished = false;
                    lastProcessingLog.FinishedOn = null;
                    _repository.Update<WorkFlowProcessingLog, Guid>(lastProcessingLog);
                }

                // Create reopen log
                var reopenLog = new LitigationRequestReopenLog
                {
                    Id = Guid.NewGuid(),
                    LitigationRequestId = litigationRequest.Id,
                    ReopenReason = requestPayload.ReopenReason,
                    PreviousInternalStatusId = previousInternalStatus,
                    NewInternalStatusId = newInternalStatus,
                    PreviousStage = previousStage,
                    NewStage = newStage,
                    PreviousSubStage = previousSubStage,
                    NewSubStage = newSubStage,
                    ReopenedOn = DateTime.Now,
                    ReopenedBy = referenceId,
                    CreatedOn = DateTime.Now,
                    CreatedBy = referenceId
                };
                await _repository.AddAsync<LitigationRequestReopenLog, Guid>(reopenLog);

                // Create new invoice if needed (unpaid fees scenarios)
                await CreateNewInvoiceIfNeeded(litigationRequest, newInternalStatus);

                // Add workflow log
                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    RequestId = litigationRequest.Id,
                    ShowForApplicant = true,
                    LogMessage = $"تم إعادة فتح الطلب بواسطة مدير النظام - السبب: {requestPayload.ReopenReason}"
                });

                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send notifications
                try
                {
                    await SendReopenNotifications(litigationRequest);
                }
                catch { }

                return await GetLitigationRequestReopenInfo(litigationRequest.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                return new BaseResponse { IsDone = false, Message = ex.Message };
            }
        }

        #region Private Helpers

        /// <summary>
        /// Determine the target status, stage, and substage based on reopen scenarios
        /// </summary>
        private (RequestStatus newInternalStatus, LitigationRequestStage? newStage, LitigationRequestSubStage? newSubStage) DetermineReopenTarget(LitigationRequest request)
        {
            // Scenario 1: Paid by beneficiary + (Cancelled or Rejected) → Review stage
            if (request.IsPaid || request.IsLitigationFeesHaveBeenCollected)
            {
                return (RequestStatus.LitigationFeesHaveBeenCollected, LitigationRequestStage.Review, null);
            }

            // Scenario 2: Review stage + Admin fees unpaid + Cancelled → Admin fees payment
            if (request.LitigationRequestStage == LitigationRequestStage.Review
                && !request.IsLitigationFeesHaveBeenCollected)
            {
                return (RequestStatus.CollectionOfLitigationFeesRequestIsBeingSent, LitigationRequestStage.Review, null);
            }

            // Scenario 3: Reconciliation stage + Reconciliation fees unpaid + Cancelled → Reconciliation fees payment
            if (request.LitigationRequestStage == LitigationRequestStage.Reconciliation
                && !request.IsReconciliationFeesHaveBeenCollected)
            {
                return (RequestStatus.CollectionOfReconciliationFeesRequestIsBeingSent, LitigationRequestStage.Reconciliation, null);
            }

            // Scenario 4: Arbitration stage + Arbitration fees unpaid + Cancelled → Set arbitrators count + Arbitration fees payment
            if (request.LitigationRequestStage == LitigationRequestStage.Arbitration
                && !request.IsArbitrationFeesHaveBeenCollected)
            {
                return (RequestStatus.WaitingForDetermineArbitratorsNumber, LitigationRequestStage.Arbitration, LitigationRequestSubStage.LitigationRequestAddArbitratorCount);
            }

            // Default: Return to Review stage
            return (RequestStatus.LitigationFeesHaveBeenCollected, LitigationRequestStage.Review, null);
        }

        /// <summary>
        /// Create a new invoice when reopening with unpaid fees
        /// </summary>
        private async Task CreateNewInvoiceIfNeeded(LitigationRequest litigationRequest, RequestStatus newStatus)
        {
            string? fees = null;
            string? serviceId = null;

            if (newStatus == RequestStatus.CollectionOfLitigationFeesRequestIsBeingSent)
            {
                fees = litigationRequest.Fees > 0 ? litigationRequest.Fees.ToString("0.#") : null;
                serviceId = _settingService.GetBySettingKey(SysSettingKeyConstants.Administrative_Fees_Billing).Result.Data?.Value;
                litigationRequest.IsCollectionOfLitigationFeesRequestIsBeingSent = true;
            }
            else if (newStatus == RequestStatus.CollectionOfReconciliationFeesRequestIsBeingSent)
            {
                fees = _settingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Fees).Result.Data?.Value;
                serviceId = _settingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Fees_Billing).Result.Data?.Value;
                litigationRequest.IsCollectionOfReconciliationFeesRequestIsBeingSent = true;
            }
            else if (newStatus == RequestStatus.WaitingForDetermineArbitratorsNumber)
            {
                // Arbitration fees invoice will be created after arbitrators count is determined
                // by the arbitration admin flow, so no invoice here
                return;
            }

            if (string.IsNullOrEmpty(fees) || string.IsNullOrEmpty(serviceId))
                return;

            var invoiceResponse = await CreateIntegratedInvoice(litigationRequest, fees, serviceId);
            if (invoiceResponse.IsDone)
            {
                var invoiceData = invoiceResponse.Data;

                var litigationRequestInvoice = new LitigationRequestInvoice
                {
                    Id = Guid.NewGuid(),
                    ReferenceId = invoiceData.Id,
                    LitigationRequestId = litigationRequest.Id,
                    InvDateJ = DateTime.TryParse(invoiceData.InvDateJ, out var invDateJ) ? invDateJ : (DateTime?)null,
                    InvDueDate = DateTime.TryParse(invoiceData.InvDueDate, out var invDueDate) ? invDueDate : (DateTime?)null,
                    SadadNumber = invoiceData.SadadNumber,
                    SadadStatus = invoiceData.SadadStatus,
                    InvoiceType = invoiceData.InvoiceType,
                    InvDateH = invoiceData.InvDateH,
                    InvTime = invoiceData.InvTime,
                    Description = invoiceData.Description,
                    IsDeleted = invoiceData.IsDeleted,
                    DeletedReason = invoiceData.DeletedReason,
                    Reference = invoiceData.Reference,
                    Source = invoiceData.Source,
                    Project = invoiceData.Project,
                    Client = invoiceData.Client,
                    CreatedBy = invoiceData.CreatedBy,
                    InvoicedNumber = invoiceData.InvoicedNumber,
                    ProvinceName = invoiceData.ProvinceName,
                    CityName = invoiceData.CityName,
                    NeighborhoodName = invoiceData.NeighborhoodName,
                    Price = decimal.TryParse(invoiceData.Price, out var price) ? price : (decimal?)null,
                    TotalQuantity = int.TryParse(invoiceData.TotalQuantity, out var totalQty) ? totalQty : (int?)null,
                    TotalPrice = decimal.TryParse(invoiceData.TotalPrice, out var totalPrice) ? totalPrice : (decimal?)null,
                    TotalNonTaxable = decimal.TryParse(invoiceData.TotalNonTaxable, out var totalNonTaxable) ? totalNonTaxable : (decimal?)null,
                    TotalTaxable = decimal.TryParse(invoiceData.TotalTaxable, out var totalTaxable) ? totalTaxable : (decimal?)null,
                    TotalTax = decimal.TryParse(invoiceData.TotalTax, out var totalTax) ? totalTax : (decimal?)null,
                    TotalTotalAfterTax = decimal.TryParse(invoiceData.TotalTotalAfterTax, out var totalAfterTax) ? totalAfterTax : (decimal?)null,
                    CreatedOn = DateTime.UtcNow,
                    StatusId = 0,
                    serviceId = serviceId
                };
                await _repository.AddAsync<LitigationRequestInvoice, Guid>(litigationRequestInvoice);
            }
        }

        public async Task<DataResponse<InvoiceDataDto>> CreateIntegratedInvoice(LitigationRequest litigation, string fees, string serviceId)
        {
            var accessToken = (await _gEELoginService.Login()).Data?.Access_token;
            try
            {
                var apiUrl = _settingService.GetBySettingKey(SysSettingKeyConstants.CreateInvoice_URL).Result.Data?.Value;

                var _httpClient = new HttpClient();

                var requestBody = new CreateInvoiceRequest
                {
                    InvoiceMasterDto = new InvoiceMasterDto
                    {
                        SourceID = 1,
                        InvDueDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        ClientNumber = 0,
                        ProjectID = 0,
                        Description = "المركز السعودى  للتحكيم العقارى"
                    },
                    InvoiceTransactionDto = new List<InvoiceTransactionDto>
                    {
                        new InvoiceTransactionDto
                        {
                            ServiceID = int.Parse(serviceId),
                            Price = int.Parse(fees),
                            Quantity = 1
                        }
                    },
                    TypeOfInvoice = 3,
                    ClientReferenceId = "A10002",
                    ArName = litigation.ApplicantCitizen?.FullName,
                    Type = litigation.ApplicantCitizen?.IdentityNumber?.ToString()?.StartsWith("1") == true ? "1" : "2",
                    IdentityNumber = litigation.ApplicantCitizen?.IdentityNumber,
                    MobileNumber = litigation.ApplicantCitizen?.Mobile,
                    Email = litigation.ApplicantCitizen?.Email,
                    CaseNumber = "ACaseNu10XXX1",
                    CaseDescription = "Reopen Fees",
                    RefIdPltfrm = "LR-" + litigation.RequestCode
                };

                var jsonString = JsonSerializer.Serialize(requestBody);
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
                {
                    Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return new DataResponse<InvoiceDataDto>() { Data = null, IsDone = false };
                }

                var resultJson = await response.Content.ReadAsStringAsync();
                var invoiceResponse = JsonSerializer.Deserialize<CreateInvoiceResponseDto>(resultJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (invoiceResponse == null || invoiceResponse.Data == null || string.IsNullOrEmpty(invoiceResponse.Data.Id))
                {
                    return new DataResponse<InvoiceDataDto>() { Data = null, IsDone = false };
                }

                if (invoiceResponse.ApiMessage?.StatusCode != 200)
                {
                    return new DataResponse<InvoiceDataDto>() { Data = null, IsDone = false };
                }

                return new DataResponse<InvoiceDataDto>() { Data = invoiceResponse.Data, IsDone = true };
            }
            catch (Exception ex)
            {
                return new DataResponse<InvoiceDataDto>() { Data = null, IsDone = false, Message = ex.Message };
            }
        }

        private string GetInvoicePaymentStatus(List<SysInvoice> invoices, InvoiceType type)
        {
            var invoice = invoices.Where(x => x.InvoiceTypeId == type).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
            if (invoice == null) return "غير متوفرة";
            return invoice.InvoiceStatusId == InvoiceStatus.Paid ? "تم السداد" : "غير مسددة";
        }

        private static string GetCategoryName(LitigationRequestCategory? category)
        {
            return category switch
            {
                LitigationRequestCategory.Reconciliation => "صلح",
                LitigationRequestCategory.Arbitration => "تحكيم",
                LitigationRequestCategory.ReconciliationAndArbitration => "صلح وتحكيم",
                _ => "غير محدد"
            };
        }

        private async Task<string?> GetStatusName(RequestStatus status)
        {
            var statusEntity = (await _repository.GetQueryable<SysLitigationRequestStatus, RequestStatus>())
                .FirstOrDefault(x => x.Id == status);
            return statusEntity?.RequestStatus;
        }

        private async Task SendReopenNotifications(LitigationRequest litigationRequest)
        {
            var msg = $"تم إعادة فتح الدعوى رقم {litigationRequest.RequestCode}";
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

            // Notify applicant
            if (litigationRequest.ApplicantCitizenId.HasValue)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = litigationRequest.ApplicantCitizenId.Value,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.LitigationRequest,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    Message = msg
                });
                if (!string.IsNullOrEmpty(litigationRequest.ApplicantCitizen?.Email))
                    emails.Add(litigationRequest.ApplicantCitizen.Email);
            }

            // Notify first party
            var firstParty = litigationRequest.Contract?.FirstPartyCitizen;
            if (firstParty != null && firstParty.Id != litigationRequest.ApplicantCitizenId)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = firstParty.Id,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.LitigationRequest,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    Message = msg
                });
                if (!string.IsNullOrEmpty(firstParty.Email))
                    emails.Add(firstParty.Email);
            }

            // Notify second party
            var secondParty = litigationRequest.Contract?.SecondPartyCitizen;
            if (secondParty != null)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = secondParty.Id,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.LitigationRequest,
                    ActorTypeId = NotificationActorType.Defendant,
                    Message = msg
                });
                if (!string.IsNullOrEmpty(secondParty.Email))
                    emails.Add(secondParty.Email);
            }

            // Notify reformer if assigned
            if (litigationRequest.ReformerCitizenId.HasValue)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = litigationRequest.ReformerCitizenId.Value,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.LitigationRequest,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    Message = msg
                });
                if (!string.IsNullOrEmpty(litigationRequest.ReformerCitizen?.Email))
                    emails.Add(litigationRequest.ReformerCitizen.Email);
            }

            // Notify arbitrator if assigned
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

            // Notify review admin
            var reviewAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Review);
            if (reviewAdmin.IsDone && reviewAdmin.Data != null)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = (Guid)reviewAdmin.Data.Id,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.LitigationRequest,
                    ActorTypeId = NotificationActorType.Employee,
                    Message = msg
                });
                if (!string.IsNullOrEmpty(reviewAdmin.Data.Email))
                    emails.Add(reviewAdmin.Data.Email);
            }

            if (notifications.Any())
                await _notificationService.SendNotifications(new SendEmailDto
                {
                    To = emails,
                    Subject = "إعادة فتح دعوى",
                    Body = body
                }, null, notifications);
        }

        #endregion
    }
}
