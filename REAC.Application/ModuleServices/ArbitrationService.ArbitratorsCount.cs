using Core.GenericRepository.Shared.Pagging;
using Core.Log.Application.Contracts.Dto;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Integration.Invoice;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.System.Invoice;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace REAC.Application.ModuleServices
{
    public partial class ArbitrationService
    {
        #region ArbitratorsCount
        public async Task<BaseResponse> AddArbitratorsCount(ArbitratorCountRequestPayload arbitratorCountRequestPayload, CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                    .Include(x => x.LitigationRequestAmount)
                    .Include(x => x.ApplicantCitizen)
                    .Include(x => x.Contract)
                    .ThenInclude(x => x.FirstPartyCitizen)
                    .Include(x => x.Contract)
                    .ThenInclude(x => x.SecondPartyCitizen)
                    .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                    .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                     .FirstOrDefault(x => x.Id == arbitratorCountRequestPayload.RequestId);

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                // Validate if the user is allowed to add arbitrators count
                if (litigationRequest.ArbitratorsSelectionDurationDateTime is not null)
                {
                    var timePassedInHours = (DateTime.Now - litigationRequest.ArbitratorsSelectionDurationDateTime).Value.TotalHours;

                    if (timePassedInHours < litigationRequest.ArbitratorSelectionDuration && (litigationRequest.FirstPartyArbitratorsCount is null || litigationRequest.SecondPartyArbitratorsCount is null))
                    {
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "برجاء الانتظار حتي اختيار الاطراف او انقضاء مدة الاختيار"
                        };
                    }
                }

                bool allowAdd = true;

                if (litigationRequest.ArbitratorsCount.HasValue)
                    allowAdd = false;
                if (!litigationRequest.FirstPartyArbitratorsCount.HasValue & !litigationRequest.SecondPartyArbitratorsCount.HasValue)
                    allowAdd = false;


                int LitigationRequestArbitratorCountSelectionDecisionPeriod = 0;
                int.TryParse(_settingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitratorCountSelectionDecisionPeriod).Result.Data?.Value, out LitigationRequestArbitratorCountSelectionDecisionPeriod);

                if (LitigationRequestArbitratorCountSelectionDecisionPeriod > 0)
                {
                    double totalHours = 0;

                    totalHours += (DateTime.Now - litigationRequest.ArbitratorsSelectionDurationDateTime).Value.TotalHours;
                    if (totalHours >= LitigationRequestArbitratorCountSelectionDecisionPeriod)
                    {
                        allowAdd = true;
                    }
                }

                if (allowAdd)
                {
                    litigationRequest.ArbitratorsCount = arbitratorCountRequestPayload.ArbitratorCount;
                    litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAddArbitratorListNomination;
                    litigationRequest.ArbitratorsCountDateTime = DateTime.Now;

                    litigationRequest.ExternalStatusId = RequestStatus.CollectionOfArbitrationFeesRequestIsBeingSent;
                    litigationRequest.InternalStatusId = RequestStatus.CollectionOfArbitrationFeesRequestIsBeingSent;

                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationRequest.Id,
                        ShowForApplicant = false,
                        LogMessage = "تم تحديد عدد هيئة التحكيم النهائى" + arbitratorCountRequestPayload.ArbitratorCount + " محكم"
                    });

                    #region اضافة فاتورة 
                    if (litigationRequest.IsExemptionReduction == false)
                    {
                        var addInvoice = await AddInvoice(litigationRequest);

                        if (!addInvoice.IsDone)
                            return addInvoice;
                    }
                    #endregion



                    var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;
                    try
                    {
                        // Send Notification
                        var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                        if (arbitrationAdmin.IsDone && arbitrationAdmin.Data != null)
                        {

                            #region تم الإنتهاء من تحديد عدد المحكمين النهائي
                            var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitrators_Count_Added).Result.Data?.Value;
                            msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            string body;
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitrators_Count_Added).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            await SendNotification(litigationRequest, RequestType.LitigationRequest, arbitrationAdmin.Data, "عدد المحكمين النهائي", msg, body, true);
                            #endregion


                            #region ارسال اشعار بالفاتورة لسدادها
                            var invoice_CreatedInvoiceMessage = _settingService.GetBySettingKey(SysSettingKeyConstants.Invoice_CreatedInvoiceMessage).Result.Data?.Value;
                            if (!string.IsNullOrWhiteSpace(invoice_CreatedInvoiceMessage))
                            {
                                invoice_CreatedInvoiceMessage = invoice_CreatedInvoiceMessage?
                                    .Replace("{LitigationCode}", litigationRequest.RequestCode)
                                    .Replace("{InvoiceCode}", addInvoice.Data.InvoiceCode);
                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                {
                                    body = await streamReader.ReadToEndAsync();
                                    body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.Invoice_CreatedInvoiceMessage).Result.Data?.Value);
                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode).Replace("{InvoiceCode}", addInvoice.Data.InvoiceCode); ;
                                }
                                await SendNotification(litigationRequest, RequestType.Invoices, arbitrationAdmin.Data, "فاتورة رسوم التحكيم", invoice_CreatedInvoiceMessage, body, false);
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لا يمكن إضافة عدد المحكمين"
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
            return await GetLitigationRequestForAddArbitratorsCount(arbitratorCountRequestPayload.RequestId, cancellationToken);
        }

        async Task<DataResponse<SavedInvoiceDto>> AddInvoice(LitigationRequest litigationRequest)
        {
            //رسوم التحكيم
            decimal arbitrationFees = default;
            if (litigationRequest.LitigationRequestAmount != null && litigationRequest.DisputeAmount != null)
            {
                arbitrationFees = await CalculateArbitrationFees(litigationRequest.ArbitratorsCount.Value, litigationRequest.DisputeAmount.Value, litigationRequest.LitigationRequestAmount);
            }
            else
            {


                var LitigationRequestAmount = (await _repository.GetQueryable<SysLitigationRequestAmount, Guid>())
                                                .Where(x => x.FromAmount == 0)
                                                .FirstOrDefault();

                if (LitigationRequestAmount != null)
                {
                    var litigationRequestFees = (await _repository.GetQueryable<LitigationRequestFees, Guid>())
                                               .Where(x => x.AmountId == LitigationRequestAmount.Id && x.ArbitratorsCount == litigationRequest.ArbitratorsCount).FirstOrDefault();
                    arbitrationFees = litigationRequestFees.ArbitrationFees;
                }


            }
            //الضريبة
            decimal taxPercentage = 0;
            var sysSetting = await _settingService.GetBySettingKey(SysSettingKeyConstants.Invoice_Taxes);
            if (sysSetting != null && sysSetting.Data != null)
            {
                decimal.TryParse(sysSetting.Data.Value, out taxPercentage);
            }
            decimal arbitrationTaxes = arbitrationFees * taxPercentage;

            decimal TotalFees = arbitrationFees + arbitrationTaxes;
            #region اضافة فاتورة


            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.Now,
                RequestId = litigationRequest.Id,
                ShowForApplicant = false,
                LogMessage = "تم انشاء فاتورة لسداد رسوم التحكيم"
            });


            #endregion

            #region Add APGI Integration Invoice 
            var Arbitration_ServiceId = "";
            if (litigationRequest.ArbitratorsCount == 1)
            {
                Arbitration_ServiceId = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_single_arbitrator_Fees_Billing).Result.Data?.Value;
            }
            else
            {
                Arbitration_ServiceId = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_multiple_arbitrator_Fees_Billing).Result.Data?.Value;
            }

            var invoiceResponse = await CreateIntegratedInvoice(litigationRequest, TotalFees.ToString(), Arbitration_ServiceId);

            if (invoiceResponse.IsDone)
            {
                var invoiceData = invoiceResponse.Data;

                var litigationRequestInvoice = new LitigationRequestInvoice
                {
                    Id = Guid.NewGuid(),
                    ReferenceId = invoiceData.Id,
                    LitigationRequestId = litigationRequest.Id,
                    // Map date fields with appropriate parsing
                    InvDateJ = DateTime.TryParse(invoiceData.InvDateJ, out var invDateJ) ? invDateJ : (DateTime?)null,
                    InvDueDate = DateTime.TryParse(invoiceData.InvDueDate, out var invDueDate) ? invDueDate : (DateTime?)null,

                    // Map string fields directly
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

                    // Map numeric fields with appropriate parsing
                    Price = decimal.TryParse(invoiceData.Price, out var price) ? price : (decimal?)null,
                    TotalQuantity = int.TryParse(invoiceData.TotalQuantity, out var totalQty) ? totalQty : (int?)null,
                    TotalPrice = decimal.TryParse(invoiceData.TotalPrice, out var totalPrice) ? totalPrice : (decimal?)null,
                    TotalNonTaxable = decimal.TryParse(invoiceData.TotalNonTaxable, out var totalNonTaxable) ? totalNonTaxable : (decimal?)null,
                    TotalTaxable = decimal.TryParse(invoiceData.TotalTaxable, out var totalTaxable) ? totalTaxable : (decimal?)null,
                    TotalTax = decimal.TryParse(invoiceData.TotalTax, out var totalTax) ? totalTax : (decimal?)null,
                    TotalTotalAfterTax = decimal.TryParse(invoiceData.TotalTotalAfterTax, out var totalAfterTax) ? totalAfterTax : (decimal?)null,

                    // Set audit fields
                    CreatedOn = DateTime.UtcNow,
                    StatusId = 0,
                    serviceId = Arbitration_ServiceId
                };
                await _repository.AddAsync<LitigationRequestInvoice, Guid>(litigationRequestInvoice);
            }
            #endregion
            return await _sysInvoiceService.AddInvoice(new CreateInvoiceDto
            {
                Amount = arbitrationFees,
                Taxes = arbitrationTaxes,
                TotalAmount = TotalFees,
                InvoiceTypeId = InvoiceType.ArbitrationFees,
                RequestId = litigationRequest.Id,
                ApplicantCitizenId = (Guid)litigationRequest.ApplicantCitizenId,
            });

        }


        #region Invoice
        public async Task<DataResponse<InvoiceDataDto>> CreateIntegratedInvoice(LitigationRequest litigation, string fees, string serviceId)
        {
            var accessToken = (await _gEELoginService.Login()).Data?.Access_token;
            try

            {



                var apiUrl = _settingService.GetBySettingKey(SysSettingKeyConstants.CreateInvoice_URL).Result.Data?.Value;
                var Administrative_ServiceId = _settingService.GetBySettingKey(SysSettingKeyConstants.Administrative_Fees_Billing).Result.Data?.Value;

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
                        //Price = (int)decimal.Parse(fees.Replace('٫', ','), new CultureInfo("de-DE")),
                        Price = int.Parse(fees) ,
                        Quantity = 1
                    },
                    //new InvoiceTransactionDto
                    //{
                    //    ServiceID = 1970,
                    //    Price = int.Parse(fees) ,
                    //    Quantity = 0
                    //}
                },
                    TypeOfInvoice = 3,
                    ClientReferenceId = "A10002",
                    ArName = litigation.ApplicantCitizen?.FullName,
                    Type = litigation.ApplicantCitizen?.IdentityNumber?.ToString()?.StartsWith("1") == true ? "1" : "2",
                    IdentityNumber = litigation.ApplicantCitizen?.IdentityNumber,
                    MobileNumber = litigation.ApplicantCitizen?.Mobile,
                    Email = litigation.ApplicantCitizen?.Email,
                    CaseNumber = "ACaseNu10XXX1",
                    CaseDescription = "Arbitration Fees",
                    RefIdPltfrm = "A-" + litigation.RequestCode
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
                    return new DataResponse<InvoiceDataDto>() { Data = null, IsDone = false }
                    ;
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

                // Check API status
                if (invoiceResponse.ApiMessage?.StatusCode != 200)
                {
                    return new DataResponse<InvoiceDataDto>() { Data = null, IsDone = false };
                }
                //var Data = new InvoiceDataDto
                //{
                //    Id = "cd91a980-caa2-4a44-b4d1-06e2aabf7049",
                //    InvDateJ = null,
                //    SadadNumber = "0",
                //    SadadStatus = "غير مسددة",
                //    InvoiceType = "مركز تحكيم",
                //    InvDateH = "1447-6-2",
                //    InvTime = "16.52.51",
                //    Description = "المركز السعودى  للتحكيم العقارى",
                //    IsDeleted = "No",
                //    DeletedReason = "",
                //    Reference = "",
                //    Source = "الهيئة العامة للعقار",
                //    Project = "",
                //    Client = "عبدالله حمد إبراهيم حمد",
                //    CreatedBy = "Arbitration platform",
                //    InvDueDate = "2025-11-23T00:00:00.0000000",
                //    InvoicedNumber = "",
                //    ProvinceName = "",
                //    CityName = "",
                //    NeighborhoodName = "",
                //    Price = "400.00",
                //    TotalQuantity = "1",
                //    TotalPrice = "400.00",
                //    TotalNonTaxable = "400.00",
                //    TotalTaxable = "0.00",
                //    TotalTax = "0.00",
                //    TotalTotalAfterTax = "400.00"
                //};
                // Return success response with invoice data
                return new DataResponse<InvoiceDataDto>() { Data = invoiceResponse.Data, IsDone = true };

            }
            catch (Exception ex)
            {
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database,
                     new LogDto()
                     {
                         Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                         Message = $"CreateIntegratedInvoice Exception ====> {ex}"
                     });
                return new DataResponse<InvoiceDataDto>() { Data = null, IsDone = false };
            }
        }
        #endregion




        async Task SendNotification(LitigationRequest litigationRequest, RequestType requestType, SysEmployee arbitrationAdmin, string subject, string message, string mailMsg, bool defendant)
        {
            List<SysNotificationDto> notifications = new List<SysNotificationDto>()
            {
                new SysNotificationDto
                {
                    ActorId = litigationRequest.ApplicantCitizen.Id,
                    RequestId=litigationRequest.Id,
                    CreatedOn= DateTime.Now,
                    RequestTypeId=requestType,
                    ActorTypeId=NotificationActorType.Plaintiff,
                    Message=message
                },
               new SysNotificationDto
               {
                    ActorId = (Guid)arbitrationAdmin?.Id,
                    RequestId=litigationRequest.Id,
                    CreatedOn= DateTime.Now,
                    RequestTypeId=requestType,
                    ActorTypeId=NotificationActorType.Employee,
                    Message=message
               }
            };

            List<string> emails = new List<string>() {
                                litigationRequest.ApplicantCitizen.Email,
                                  arbitrationAdmin?.Email
                                };

            List<string> recipients = new List<string>()
                            {
                                litigationRequest.ApplicantCitizen.Mobile,
                                  arbitrationAdmin?.Mobile
                            };
            if (litigationRequest?.Contract?.FirstPartyCitizenId != null && litigationRequest?.Contract?.FirstPartyCitizenId != litigationRequest.ApplicantCitizenId)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = (Guid)litigationRequest.Contract.FirstPartyCitizenId,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = requestType,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    Message = message
                });
                emails.Add(litigationRequest.Contract.FirstPartyCitizen.Email);
                recipients.Add(litigationRequest.Contract.FirstPartyCitizen.Mobile);


            }
            ;

            if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility.CitizenId != litigationRequest.ApplicantCitizenId)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.CitizenId,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = requestType,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    Message = message,
                });
                emails.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Email);
                recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
            }
            ;

            if (defendant)
            {
                if (litigationRequest?.Contract?.SecondPartyCitizenId != null)
                {
                    notifications.Add(new SysNotificationDto
                    {
                        ActorId = (Guid)litigationRequest.Contract.SecondPartyCitizenId,
                        RequestId = litigationRequest.Id,
                        CreatedOn = DateTime.Now,
                        RequestTypeId = requestType,
                        ActorTypeId = NotificationActorType.Defendant,
                        Message = message
                    });
                    emails.Add(litigationRequest.Contract.SecondPartyCitizen.Email);
                    recipients.Add(litigationRequest.Contract.SecondPartyCitizen.Mobile);
                }
                ;

                if (litigationRequest?.Contract?.SecondPartyFacilityId != null)
                {
                    notifications.Add(new SysNotificationDto
                    {
                        ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.CitizenId,
                        RequestId = litigationRequest.Id,
                        CreatedOn = DateTime.Now,
                        RequestTypeId = requestType,
                        ActorTypeId = NotificationActorType.Defendant,
                        Message = message
                    });
                    emails.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Email);
                    recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);
                }
                ;
            }
            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
            {
                To = emails,
                Subject = subject,
                Body = mailMsg
            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
            {
                message_text = message,
                recipients = recipients,
                notification_type = "SMS",

            }, notifications);
        }


        public async Task<decimal> CalculateArbitrationFees(int arbitratorsCount, decimal disputeAmount, SysLitigationRequestAmount litigationRequestAmount)
        {
            decimal amount = 0;

            var litigationRequestFees = (await _repository.GetQueryable<LitigationRequestFees, Guid>())
               .FirstOrDefault(x => x.ArbitratorsCount == arbitratorsCount &&
               x.AmountId == litigationRequestAmount.Id);

            if (litigationRequestFees != null)
            {
                amount = litigationRequestFees.ArbitrationFees;
                if (litigationRequestFees.PercentageExtraAmount.HasValue && litigationRequestFees.ExtraAmount.HasValue && disputeAmount > litigationRequestFees.ExtraAmount)
                {
                    decimal exteraAmount = disputeAmount - (decimal)litigationRequestFees.ExtraAmount;
                    if (exteraAmount > 0)
                    {
                        amount += exteraAmount * (litigationRequestFees.PercentageExtraAmount.Value / 100);
                    }
                }
            }


            return amount;
        }

        public async Task<BaseResponse> GetLitigationRequestForAddArbitratorsCount(Guid requestId, CancellationToken cancellationToken)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.Area)
                         .Include(x => x.City)
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

                         .Where(x => x.Id == requestId).AsNoTracking().ToList().FirstOrDefault();

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);


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
                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                {
                    allowAdd = false;
                }
                else
                {
                    if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorCount)
                        allowAdd = false;
                }
            }
            if (litigationRequest.ArbitratorsCount.HasValue)
            {
                allowAdd = false;
            }
            return new DataResponse<LitigationRequestForAllArbitratorCountDto>()
            {

                Data = new LitigationRequestForAllArbitratorCountDto()
                {
                    AllowAdd = allowAdd,
                    LitigationRequestShortDto = request,
                    ArbitratorsCount = litigationRequest?.ArbitratorsCount,
                    FirstPartyArbitratorsCount = litigationRequest?.FirstPartyArbitratorsCount,
                    SecondPartyArbitratorsCount = litigationRequest?.SecondPartyArbitratorsCount,
                },
                IsDone = true,
            };

        }
        public async Task<BaseResponse> GetLitigationRequestForAddArbitratorsCount(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)

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
                                      .Include(x => x.Contract.SecondPartyCitizen).Include(x => x.Contract.LitigationRequestContractType);


            litigationRequests = entitiesQuerable.Where(
                x => x.RequestCode != null & x.ArbitratorSelectionDuration.HasValue
            & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation
            & x.LitigationRequestStage == LitigationRequestStage.Arbitration)
                                       .OrderByDescending(x => x.CreatedOn)
                                       .AsSplitQuery()
                                       .AsNoTracking()
                                       .GetPaged(pageIndex, pageSize);

            List<LitigationRequestMircoForAllArbitratorsCountDto> litigationRequestMircoDtos = new List<LitigationRequestMircoForAllArbitratorsCountDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    LitigationRequestMircoForAllArbitratorsCountDto litigationRequestMircoDto = new LitigationRequestMircoForAllArbitratorsCountDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.ArbitratorsCount = litigationRequest.ArbitratorsCount;
                    litigationRequestMircoDto.FirstPartyArbitratorsCount = litigationRequest.FirstPartyArbitratorsCount;
                    litigationRequestMircoDto.SecondPartyArbitratorsCount = litigationRequest.SecondPartyArbitratorsCount;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.DefendantCitizen == null) ? litigationRequest.DefendantFacility?.ArabicFacilityName : litigationRequest.DefendantCitizen?.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.IsArbitrationFeesHaveBeenCollected = litigationRequest.IsArbitrationFeesHaveBeenCollected;
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
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                            allowAdd = false;
                        else
                        {

                            if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorCount)
                                allowAdd = false;
                        }
                    }

                    if (!litigationRequest.FirstPartyArbitratorsCount.HasValue & !litigationRequest.SecondPartyArbitratorsCount.HasValue)
                        allowAdd = false;

                    if (!litigationRequest.ArbitratorsCount.HasValue)
                        allowAdd = false;

                    if (allowAdd)
                        litigationRequestMircoDto.IsDone = "--";
                    else if (!litigationRequest.FirstPartyArbitratorsCount.HasValue || !litigationRequest.SecondPartyArbitratorsCount.HasValue)
                    {
                        litigationRequestMircoDto.IsDone = "--";
                    }
                    else
                        litigationRequestMircoDto.IsDone = "تم";

                    litigationRequestMircoDto.Status = litigationRequest.InternalStatus.RequestStatus;
                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                }
            }

            return new DataResponse<PagedResult<LitigationRequestMircoForAllArbitratorsCountDto>>
            {
                Data =
                  new PagedResult<LitigationRequestMircoForAllArbitratorsCountDto>
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

        public async Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsCountReply(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)

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
                                      .Include(x => x.Contract.SecondPartyCitizen).Include(x => x.Contract.LitigationRequestContractType);

            litigationRequests = entitiesQuerable.Where
                                      (x => x.RequestCode != null & x.ArbitratorSelectionDuration.HasValue
                                        & x.ApplicantCitizenId == referenceId & x.LitigationRequestStage == LitigationRequestStage.Arbitration & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      //| x.RequestCode != null & x.ArbitratorSelectionDuration.HasValue & x.Contract.FirstPartyCitizenId == referenceId & x.LitigationRequestStage == LitigationRequestStage.Arbitration & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      | x.RequestCode != null & x.ArbitratorSelectionDuration.HasValue & x.Contract.SecondPartyCitizenId == referenceId & x.LitigationRequestStage == LitigationRequestStage.Arbitration & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      | x.RequestCode != null & x.ArbitratorSelectionDuration.HasValue & x.Contract.FirstPartyFacility.CitizenId == referenceId & x.LitigationRequestStage == LitigationRequestStage.Arbitration & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      | x.RequestCode != null & x.ArbitratorSelectionDuration.HasValue & x.Contract.SecondPartyFacility.CitizenId == referenceId & x.LitigationRequestStage == LitigationRequestStage.Arbitration & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      )
                                       .OrderByDescending(x => x.CreatedOn)
                                       .AsSplitQuery()
                                       .AsNoTracking()
                                       .GetPaged(pageIndex, pageSize);


            List<LitigationRequestMircoForArbitratorsCountDto> litigationRequestMircoDtos = new List<LitigationRequestMircoForArbitratorsCountDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    LitigationRequestMircoForArbitratorsCountDto litigationRequestMircoDto = new LitigationRequestMircoForArbitratorsCountDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.DefendantCitizen == null) ? litigationRequest.DefendantFacility?.ArabicFacilityName : litigationRequest.DefendantCitizen?.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                    bool allowAdd = true;
                    bool IsDone = false;
                    if (litigationRequest.IsFinished.HasValue)
                    {
                        if (litigationRequest.IsFinished.Value)
                        {
                            allowAdd = false;
                            IsDone = true;
                        }
                    }
                    if (!IsDone)
                    {
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                            allowAdd = false;
                        else
                        {
                            if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorCount)
                                allowAdd = false;
                        }
                    }
                    if (litigationRequest.ApplicantCitizenId == referenceId)
                    {
                        if (litigationRequest.FirstPartyArbitratorsCount.HasValue)
                        {
                            litigationRequestMircoDto.ArbitratorsCount = litigationRequest.FirstPartyArbitratorsCount.Value;
                            allowAdd = false;
                        }

                    }
                    if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                        {
                            if (litigationRequest.FirstPartyArbitratorsCount.HasValue)
                            {
                                litigationRequestMircoDto.ArbitratorsCount = litigationRequest.FirstPartyArbitratorsCount.Value;
                                allowAdd = false;
                            }
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                        {
                            if (litigationRequest.SecondPartyArbitratorsCount.HasValue)
                            {
                                litigationRequestMircoDto.ArbitratorsCount = litigationRequest.SecondPartyArbitratorsCount.Value;
                                allowAdd = false;
                            }
                        }
                    }
                    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                        {
                            if (litigationRequest.FirstPartyArbitratorsCount.HasValue)
                            {
                                litigationRequestMircoDto.ArbitratorsCount = litigationRequest.FirstPartyArbitratorsCount.Value;
                                allowAdd = false;
                            }
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                        {
                            if (litigationRequest.SecondPartyArbitratorsCount.HasValue)
                            {
                                litigationRequestMircoDto.ArbitratorsCount = litigationRequest.SecondPartyArbitratorsCount.Value;
                                allowAdd = false;
                            }
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

            return new DataResponse<PagedResult<LitigationRequestMircoForArbitratorsCountDto>>
            {
                Data =
                  new PagedResult<LitigationRequestMircoForArbitratorsCountDto>
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
        public async Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsCountReply(Guid requestId, CancellationToken cancellationToken)
        {

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.Area)
                         .Include(x => x.City)
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

                         .Where(x => x.Id == requestId
                         &&
                x.ApplicantCitizenId == referenceId
                      //|
                      //x.Contract.FirstPartyCitizenId == referenceId & x.Id == requestId
                      |
                        x.Contract.SecondPartyCitizenId == referenceId & x.Id == requestId
                      |
                       x.Contract.FirstPartyFacility.CitizenId == referenceId & x.Id == requestId
                      |
                      x.Contract.SecondPartyFacility.CitizenId == referenceId & x.Id == requestId

                         ).AsNoTracking().ToList().FirstOrDefault();

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };
            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);



            LitigationRequestMircoDto litigationRequestMircoDto = new LitigationRequestMircoDto();

            bool allowAdd = true;
            bool IsDone = false;
            int? arbitratorsCount = null;
            if (litigationRequest.IsFinished.HasValue)
            {
                if (litigationRequest.IsFinished.Value)
                {
                    allowAdd = false;
                    IsDone = true;
                }
            }
            if (!IsDone)
            {
                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                    allowAdd = false;
                else
                {
                    if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorCount)
                        allowAdd = false;
                }
            }
            if (litigationRequest.ApplicantCitizenId == referenceId)
            {
                if (litigationRequest.FirstPartyArbitratorsCount.HasValue)
                {
                    arbitratorsCount = litigationRequest.FirstPartyArbitratorsCount.Value;
                    allowAdd = false;
                }

            }
            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
            {
                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                {
                    if (litigationRequest.FirstPartyArbitratorsCount.HasValue)
                    {
                        arbitratorsCount = litigationRequest.FirstPartyArbitratorsCount.Value;
                        allowAdd = false;
                    }
                }
            }
            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
            {
                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                {
                    if (litigationRequest.SecondPartyArbitratorsCount.HasValue)
                    {
                        arbitratorsCount = litigationRequest.SecondPartyArbitratorsCount.Value;
                        allowAdd = false;
                    }
                }
            }
            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
            {
                if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                {
                    if (litigationRequest.FirstPartyArbitratorsCount.HasValue)
                    {
                        arbitratorsCount = litigationRequest.FirstPartyArbitratorsCount.Value;
                        allowAdd = false;
                    }
                }
            }
            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
            {
                if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                {
                    if (litigationRequest.SecondPartyArbitratorsCount.HasValue)
                    {
                        arbitratorsCount = litigationRequest.SecondPartyArbitratorsCount.Value;
                        allowAdd = false;
                    }
                }
            }

            return new DataResponse<LitigationRequestForArbitratorCountDto>()
            {
                Data = new LitigationRequestForArbitratorCountDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    ArbitratorsCount = arbitratorsCount
                },
                IsDone = true,
            };
        }
        public async Task<BaseResponse> AddCitizenLitigationRequestForReportArbitratorsCountReply(ArbitratorCountRequestPayload arbitratorCountRequestPayload, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                    .Include(x => x.LitigationRequestAmount)
                     .Include(x => x.ApplicantCitizen)
                    .Include(x => x.Contract)
                    .ThenInclude(x => x.FirstPartyCitizen)
                    .Include(x => x.Contract)
                    .ThenInclude(x => x.SecondPartyCitizen)
                    .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                    .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                    .FirstOrDefault(x => x.Id == arbitratorCountRequestPayload.RequestId);

            bool isDone = false;
            if (litigationRequest != null)
            {
                if (litigationRequest.ApplicantCitizen != null)
                {
                    if (litigationRequest.ApplicantCitizenId == referenceId)
                    {
                        if (!litigationRequest.FirstPartyArbitratorsCount.HasValue)
                        {
                            litigationRequest.FirstPartyArbitratorsCount = arbitratorCountRequestPayload.ArbitratorCount;
                            isDone = true;
                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم تحديد عدد هيئة التحكيم من قبل المدعى " + arbitratorCountRequestPayload.ArbitratorCount + " محكم"
                            });
                        }
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.FirstPartyCitizen != null)
                    {
                        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                        {
                            if (!litigationRequest.FirstPartyArbitratorsCount.HasValue)
                            {
                                litigationRequest.FirstPartyArbitratorsCount = arbitratorCountRequestPayload.ArbitratorCount;
                                isDone = true;
                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = "تم تحديد عدد هيئة التحكيم من قبل المدعى " + arbitratorCountRequestPayload.ArbitratorCount + " محكم"
                                });
                            }
                        }
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.SecondPartyCitizen != null)
                    {
                        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                        {
                            if (!litigationRequest.SecondPartyArbitratorsCount.HasValue)
                            {
                                litigationRequest.SecondPartyArbitratorsCount = arbitratorCountRequestPayload.ArbitratorCount;
                                isDone = true;
                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = "تم تحديد عدد هيئة التحكيم من قبل المدعى عليه " + arbitratorCountRequestPayload.ArbitratorCount + " محكم"
                                });
                            }
                        }
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.FirstPartyFacility != null)
                    {
                        if (litigationRequest.Contract.FirstPartyFacility.CitizenId == referenceId)
                        {
                            if (!litigationRequest.FirstPartyArbitratorsCount.HasValue)
                            {
                                litigationRequest.FirstPartyArbitratorsCount = arbitratorCountRequestPayload.ArbitratorCount;
                                isDone = true;
                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = "تم تحديد عدد هيئة التحكيم من قبل المدعى " + arbitratorCountRequestPayload.ArbitratorCount + " محكم"
                                });
                            }
                        }
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.SecondPartyFacility != null)
                    {
                        if (litigationRequest.Contract.SecondPartyFacility.CitizenId == referenceId)
                        {
                            if (!litigationRequest.SecondPartyArbitratorsCount.HasValue)
                            {
                                litigationRequest.SecondPartyArbitratorsCount = arbitratorCountRequestPayload.ArbitratorCount;
                                isDone = true;
                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = "تم تحديد عدد هيئة التحكيم من قبل المدعى عليه " + arbitratorCountRequestPayload.ArbitratorCount + " محكم"
                                });
                            }
                        }
                    }
                }
            }

            if (litigationRequest.FirstPartyArbitratorsCount.HasValue & litigationRequest.SecondPartyArbitratorsCount.HasValue)
            {

                if (litigationRequest.FirstPartyArbitratorsCount.Value == litigationRequest.SecondPartyArbitratorsCount.Value)
                {
                    litigationRequest.ArbitratorsCount = litigationRequest.FirstPartyArbitratorsCount;
                    litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAddArbitratorListNomination;
                    litigationRequest.ArbitratorsCountDateTime = DateTime.Now;

                    litigationRequest.ExternalStatusId = RequestStatus.CollectionOfArbitrationFeesRequestIsBeingSent;
                    litigationRequest.InternalStatusId = RequestStatus.CollectionOfArbitrationFeesRequestIsBeingSent;


                    #region اضافة فاتورة              
                    var addInvoice = await AddInvoice(litigationRequest);

                    if (!addInvoice.IsDone)
                        return addInvoice;
                    #endregion

                    // Send Notification
                    // تم الإنتهاء من تحديد عدد المحكمين النهائي لاتفاق كلا المدعى والمدعى عليه على عدد المحكمين
                    try
                    {
                        var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                        var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitrators_Count_FromBothSides).Result.Data?.Value;
                        msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        string body;
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitrators_Count_FromBothSides).Result.Data?.Value);
                            body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                        }
                        //if (litigationRequest?.Contract?.FirstPartyCitizen != null)
                        //    msg = msg?.Replace("{FirstParty}", litigationRequest?.Contract?.FirstPartyCitizen.FullName);
                        //if (litigationRequest?.Contract?.SecondPartyCitizen != null)
                        //    msg = msg?.Replace("{SecondParty}", litigationRequest?.Contract?.SecondPartyCitizen.FullName);
                        //if (litigationRequest?.Contract?.FirstPartyFacility != null)
                        //    msg = msg?.Replace("{FirstParty}", litigationRequest?.Contract?.FirstPartyFacility.Citizen.FullName);
                        //if (litigationRequest?.Contract?.SecondPartyFacility != null)
                        //    msg = msg?.Replace("{SecondParty}", litigationRequest?.Contract?.SecondPartyFacility.Citizen.FullName);


                        if (arbitrationAdmin.IsDone)
                        {
                            #region ارسال اشعار بالفاتورة لسدادها
                            var invoice_CreatedInvoiceMessage = _settingService.GetBySettingKey(SysSettingKeyConstants.Invoice_CreatedInvoiceMessage).Result.Data?.Value;
                            if (!string.IsNullOrWhiteSpace(invoice_CreatedInvoiceMessage))
                            {
                                invoice_CreatedInvoiceMessage = invoice_CreatedInvoiceMessage?
                                    .Replace("{LitigationCode}", litigationRequest.RequestCode)
                                    .Replace("{InvoiceCode}", addInvoice.Data.InvoiceCode);
                                pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                {
                                    body = await streamReader.ReadToEndAsync();
                                    body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.Invoice_CreatedInvoiceMessage).Result.Data?.Value);
                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode).Replace("{InvoiceCode}", addInvoice.Data.InvoiceCode);
                                }
                                await SendNotification(litigationRequest, RequestType.Invoices, arbitrationAdmin.Data, "فاتورة رسوم التحكيم", invoice_CreatedInvoiceMessage, body, false);
                            }
                            #endregion


                            List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
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
                                  arbitrationAdmin.Data?.Email
                                };
                            List<string> recipients = new List<string>() {
                                  arbitrationAdmin.Data?.Mobile
                                };

                            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = emails,
                                Subject = "الإنتهاء من تحديد عدد المحكمين",
                                Body = msg
                            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = recipients,
                                notification_type = "SMS",

                            }, notifications);

                        }
                    }
                    catch (Exception e)
                    {


                    }



                }
                else
                {
                    // Send Notification
                    // تم الإنتهاء من تحديد عدد المحكمين من قبل المدعى والمدعى عليه
                    try
                    {
                        var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                        var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitrators_Count_Added_AfterAgreement).Result.Data?.Value;
                        msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        string body;
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitrators_Count_Added_AfterAgreement).Result.Data?.Value);
                            body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                        }

                        if (arbitrationAdmin.IsDone)
                        {
                            List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
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
                                  arbitrationAdmin.Data?.Email
                                };
                            List<string> recipients = new List<string>() {
                                  arbitrationAdmin.Data?.Mobile
                                };

                            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = emails,
                                Subject = "الإنتهاء من تحديد عدد المحكمين",
                                Body = body
                            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = recipients,
                                notification_type = "SMS",

                            }, notifications);

                        }
                    }
                    catch (Exception e)
                    {


                    }

                }
            }

            var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            if (!repositorySaveResponse.IsDone)
                return repositorySaveResponse;

            return await GetCitizenLitigationRequestForArbitratorsCountReply(arbitratorCountRequestPayload.RequestId, cancellationToken);

        }
        #endregion

    }
}
