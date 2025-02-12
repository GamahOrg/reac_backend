using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.System.Invoice;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Invoice;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.AppServices.System
{
    public class SysInvoiceService : BasicService<SysInvoice, Guid>, ISysInvoiceService
    {
        private readonly IJwtReader _jwtReader;
        private readonly ISysSettingService _settingService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISysEmployeeService _sysEmployeeService;
        protected readonly INotificationService _notificationService;

        public SysInvoiceService(IEntityRepository repository,
            IServiceProvider serviceProvider,
            ISysEmployeeService sysEmployeeService,
            INotificationService notificationService,
            IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ISysSettingService settingService) : base(repository, mapper, loggerManager, jwtReader)
        {
            _jwtReader = jwtReader;
            _mapper = mapper;
            _settingService = settingService;
            _serviceProvider = serviceProvider;
            _sysEmployeeService = sysEmployeeService;
            _notificationService = notificationService;
        }

        public async Task<DataResponse<PagedResult<InvoiceDto>>> GetPagedList(int pageIndex, int pageSize)
        {
            Guid citizenId = Guid.Empty;
            _jwtReader.GetReferenceId(ref citizenId);

            var invoices = (await _repository.GetQueryable<SysInvoice, Guid>())
                .Where(x => x.ApplicantCitizenId == citizenId)
                .Include(x => x.InvoiceType.Translations)
                .Include(x => x.InvoiceStatus.Translations)
                .OrderByDescending(x => x.CreatedOn)
                .AsSplitQuery()
                .AsNoTracking()
                .GetPaged(pageIndex, pageSize);

            return new DataResponse<PagedResult<InvoiceDto>>
            {
                IsDone = true,
                Data = new PagedResult<InvoiceDto>
                {
                    Results = _mapper.Map<List<InvoiceDto>>(invoices.Results),
                    CurrentPage = invoices.CurrentPage,
                    PageCount = invoices.PageCount,
                    PageSize = invoices.PageSize,
                    TotalCount = invoices.TotalCount
                }
            };

        }

        public async Task<DataResponse<InvoiceDto>> GetInvoiceId(Guid id)
        {
            var invoice = (await _repository.GetQueryable<SysInvoice, Guid>()).FirstOrDefault(x => x.Id == id);
            if (invoice == null)
                return new DataResponse<InvoiceDto> { Message = "No Found" };

            return new DataResponse<InvoiceDto>
            {
                IsDone = true,
                Data = _mapper.Map<InvoiceDto>(invoice)
            };
        }

        public async Task<DataResponse<SavedInvoiceDto>> AddInvoice(CreateInvoiceDto invoiceDto, bool saveChnages = false)
        {
            var invoice = new SysInvoice
            {
                Id = Guid.NewGuid(),
                RequestId = invoiceDto.RequestId,
                Amount = invoiceDto.Amount,
                InvoiceTax = invoiceDto.Taxes,
                InvoiceStatusId = InvoiceStatus.Payable,
                InvoiceTypeId = invoiceDto.InvoiceTypeId,
                CreatedOn = DateTime.UtcNow,
                TotalAmount = invoiceDto.TotalAmount,
                CreatedBy = _jwtReader.GetUserId(),
                ApplicantCitizenId = invoiceDto.ApplicantCitizenId,
            };

            DateTime expirationDate = DateTime.UtcNow;
            var experationSetting = await _settingService.GetBySettingKey(SysSettingKeyConstants.Invoice_ExpirationDays);
            if (experationSetting != null && experationSetting.Data != null)
            {
                if (int.TryParse(experationSetting.Data.Value, out int days))
                {
                    expirationDate.AddDays(days);
                }
            }
            else
            {
                //default expiration
                expirationDate.AddDays(2);
            }

            invoice.InvoiceCode = await GenerateCode();
            invoice.ExpirationDate = expirationDate;

            await _repository.AddAsync<SysInvoice, Guid>(invoice);
            if (saveChnages)
            {
                var response = await _repository.SaveChangesAsync();
                if (response != null && response.IsDone)
                    return new DataResponse<SavedInvoiceDto> { Message = response.Message };
            }
            return new DataResponse<SavedInvoiceDto>
            {
                IsDone = true,
                Data = new SavedInvoiceDto
                {
                    Id = invoice.Id,
                    InvoiceCode = invoice.InvoiceCode,
                }
            };
        }

        public async Task<BaseResponse> PayInvoice(Guid id)
        {
            var invoice = (await _repository.GetQueryable<SysInvoice, Guid>()).FirstOrDefault(x => x.Id == id);
            if (invoice == null)
                return new BaseResponse { Message = "الفاتورة غير موجودة" };

            invoice.InvoiceStatusId = InvoiceStatus.Paid;
            invoice.PaymentDate = DateTime.UtcNow;

            _repository.Update<SysInvoice, Guid>(invoice);

            switch (invoice.InvoiceTypeId)
            {
                case InvoiceType.ArbitrationFees:
                    var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                        .Include(x => x.Contract.FirstPartyCitizen)
                        .Include(x => x.Contract.FirstPartyFacility)
                        .Include(x => x.ApplicantCitizen)
                        .FirstOrDefault(x => x.Id == invoice.RequestId);
                    if (litigationRequest == null)
                        return new BaseResponse { Message = "الطلب غير موجودة" };

                    litigationRequest.ExternalStatusId = RequestStatus.ArbitrationFeesHaveBeenCollected;
                    litigationRequest.InternalStatusId = RequestStatus.ArbitrationFeesHaveBeenCollected;
                    //litigationRequest.IsArbitrationFeesHaveBeenCollected = true;

                    _repository.Update<LitigationRequest, Guid>(litigationRequest);

                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationRequest.Id,
                        ShowForApplicant = false,
                        LogMessage = "تم دفع رسوم التحكيم"
                    });


                    #region ارسال اشعار بالفاتورة لسدادها
                    var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                    if (arbitrationAdmin.IsDone && arbitrationAdmin.Data != null)
                    {
                        var invoice_PaidInvoiceMessage = _settingService.GetBySettingKey(SysSettingKeyConstants.Invoice_ArbitrationInvoicePaidMessage).Result.Data?.Value;
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        string body;
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.Invoice_ArbitrationInvoicePaidMessage).Result.Data?.Value);
                        }
                        if (!string.IsNullOrWhiteSpace(invoice_PaidInvoiceMessage))
                        {
                            invoice_PaidInvoiceMessage = invoice_PaidInvoiceMessage?
                                .Replace("{LitigationCode}", litigationRequest.RequestCode);
                            body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            if (!string.IsNullOrWhiteSpace(invoice_PaidInvoiceMessage))
                                await SendNotification(litigationRequest, RequestType.Invoices, arbitrationAdmin.Data, "سداد رسوم التحكيم", invoice_PaidInvoiceMessage, body, false);
                        }
                    }
                    #endregion

                    //var _bpfService = _serviceProvider.GetRequiredService<IBpfService>();
                    //var _stepService = _serviceProvider.GetRequiredService<IWorkFlowStepService>();

                    //var processingLog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())
                    //    .Include(t => t.WorkFlowStep)
                    //    .Include(t => t.WorkFlowStep.WorkFlowStepEvents)
                    //    .AsNoTracking()
                    //    .Where(x => x.RequestId == litigationRequest.Id).ToList();

                    //if (processingLog.Count > 0)
                    //{
                    //    var currentProcessingLog = processingLog.Where(t => t.IsFinished == false).OrderBy(t => t.CreatedOn).ToList();
                    //    if (processingLog.Count > 0)
                    //    {
                    //        ProcessRequestPayload processRequestPayload = new ProcessRequestPayload();
                    //        processRequestPayload.RequestId = litigationRequest.Id;
                    //        processRequestPayload.ProcessingLogId = currentProcessingLog.LastOrDefault().Id;
                    //        processRequestPayload.WorkflowStepEventId = currentProcessingLog.LastOrDefault().WorkFlowStep.WorkFlowStepEvents.FirstOrDefault().Id;
                    //        processRequestPayload.Notes = "";
                    //        processRequestPayload.UploadedFilesIds = new List<Guid>();
                    //        await _bpfService.ProcessWorkFlow(processRequestPayload, default);
                    //    }
                    //}
                    break;
            }

            return await _repository.SaveChangesAsync();
        }


        public async Task SendNotification(LitigationRequest litigationRequest, RequestType requestType, SysEmployee arbitrationAdmin, string subject, string message, string mailMsg, bool defendant)
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


            };

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
            };

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
                };

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
                };
            }
            await _notificationService.SendNotifications(new SendEmailDto()
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

        private async Task<string> GenerateCode()
        {
            long lastCode = default;
            var lastInvoice = (await _repository.GetQueryable<SysInvoice, Guid>())
                            .OrderByDescending(x => x.InvoiceCode)
                            .FirstOrDefault();

            if (lastInvoice != null && !string.IsNullOrWhiteSpace(lastInvoice.InvoiceCode))
            {
                long.TryParse(lastInvoice.InvoiceCode, out lastCode);
            }

            return (lastCode + 1).ToString("D8");
        }
    }
}
