using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Core.Log.Application.Contracts.Dto;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Integration.Invoice;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowLog;
using REAC.Application.Contracts.Dtos.Wpf.System;
using REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEvent;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace Bpf.Application.ModuleServices
{
    public partial class BpfService
    {
        public async Task<BaseResponse> ProcessWorkFlow(ProcessRequestPayload requestPayload, CancellationToken cancellationToken)
        {
            ResponseWorkFlowDto responseWorkFlowDto = new ResponseWorkFlowDto();
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var processingLog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())?
                        .Where(x => x.Id == requestPayload.ProcessingLogId)
                        .Include(x => x.Employee)
                        .Include(x => x.WorkFlowStep)
                        .ThenInclude(x => x.WorkFlowStepEvents
                        .Where(x => x.Id == requestPayload.WorkflowStepEventId))
                        .Single();


                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                    .Include(x => x.WorkFlowProcessingLogs)
                    .ThenInclude(x => x.Employee)
                    .Include(x => x.Contract)
                        .ThenInclude(x => x.FirstPartyCitizen)
                        .Include(x => x.Contract)
                         .ThenInclude(x => x.SecondPartyCitizen)
                        .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                        .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                        .Include(x => x.ApplicantCitizen)
                        .Where(x => x.Id == requestPayload.RequestId)
                        .Single();

                Guid? employeeId = litigationRequest.WorkFlowProcessingLogs.Where(w => w.EmployeeId != null).OrderBy(x => x.CreatedOn).LastOrDefault()?.EmployeeId;

                #region Validate Employee

                //var employee = (await _repository.GetQueryable<SysEmployee, Guid>())?
                //    .Where(x => x.Id == employeeId).Single();

                //if (employee == null)
                //    return new BaseResponse()
                //    {
                //        IsDone = false,
                //        Message = "Employee Not Found"
                //    };


                //if (employeeId != processingLog?.Employee?.Id)
                //{
                //    if (!employee.IsAdmin)
                //        return new BaseResponse()
                //        {
                //            IsDone = false,
                //            Message = "Not Allowed"
                //        };

                //    if (!employee.WorkAsEmployee)
                //        return new BaseResponse()
                //        {
                //            IsDone = false,
                //            Message = "Not Allowed"
                //        };
                //}

                #endregion

                #region Validate processingLog
                if (processingLog == null)
                    return new BaseResponse() { IsDone = false, Message = "process log not found" };

                if (processingLog.IsFinished)
                    return new BaseResponse() { IsDone = false, Message = "Not Allowed" };
                #endregion


                #region invoice APGI integration
                //رسوم الدعوه
                if (litigationRequest.InternalStatusId == REAC.Domain.Shared.Enum.RequestStatus.New)
                {

                    var litigationStageFees = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Where(x => x.Id == requestPayload.RequestId & x.IsFinished == false)
                        .Select(x => new
                        {
                            x.LitigationRequestAmount,
                            x.LitigationRequestStage,
                            x.IsLitigationFeesHaveBeenCollected,
                            x.FeesDifference
                        }).First();

                    decimal minFees = 0;
                    if (litigationStageFees?.LitigationRequestAmount == null)
                    {
                        minFees = (await _repository.GetQueryable<SysLitigationRequestAmount, Guid>())?
                      .Where(x => x.FromAmount == 0)
                      .FirstOrDefault()?.Fees ?? 0;
                    }

                    var Fees = litigationStageFees.LitigationRequestStage == REAC.Domain.Shared.Enum.LitigationRequestStage.Review && !litigationStageFees.IsLitigationFeesHaveBeenCollected ? litigationStageFees.FeesDifference != 0 ? litigationStageFees.FeesDifference.ToString("0.#") : litigationStageFees.LitigationRequestAmount?.Fees.ToString("0.#") : null;
                    if (litigationStageFees?.LitigationRequestAmount == null)
                    {
                        Fees = litigationStageFees.LitigationRequestStage == REAC.Domain.Shared.Enum.LitigationRequestStage.Review && !litigationStageFees.IsLitigationFeesHaveBeenCollected ? minFees.ToString("0.#") : null;
                    }
                    var Administrative_ServiceId = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Administrative_Fees_Billing).Result.Data?.Value;

                    var invoiceResponse = await CreateIntegratedInvoice(litigationRequest, Fees, Administrative_ServiceId);

                    if (invoiceResponse.IsDone)
                    {
                        var invoiceData = invoiceResponse.Data;

                        var litigationRequestInvoice = new LitigationRequestInvoice
                        {
                            // Generate new Guid for the entity Id (inherited from BaseEntity<Guid>)
                            Id = Guid.NewGuid(),

                            // Map ReferenceId from the response Id
                            ReferenceId = invoiceData.Id,

                            // Set the foreign key
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
                            serviceId = Administrative_ServiceId
                        };
                        await _repository.AddAsync<LitigationRequestInvoice, Guid>(litigationRequestInvoice);
                    }
                }
                //رسوم الصلح
                if (litigationRequest.InternalStatusId == REAC.Domain.Shared.Enum.RequestStatus.LitigationApprovedCollectionOfReconciliationFeesRequestIsBeingSend)
                {
                    var reconciliationFees = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Fees).Result.Data?.Value;
                    var Administrative_ServiceId = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Fees_Billing).Result.Data?.Value;

                    //send input service id for DB
                    // save service id to DB
                    var invoiceResponse = await CreateIntegratedInvoice(litigationRequest, reconciliationFees, Administrative_ServiceId);
                    if (invoiceResponse.IsDone)
                    {
                        var invoiceData = invoiceResponse.Data;

                        var litigationRequestInvoice = new LitigationRequestInvoice
                        {
                            // Generate new Guid for the entity Id (inherited from BaseEntity<Guid>)
                            Id = Guid.NewGuid(),

                            // Map ReferenceId from the response Id
                            ReferenceId = invoiceData.Id,

                            // Set the foreign key
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
                            serviceId= Administrative_ServiceId
                        };
                        await _repository.AddAsync<LitigationRequestInvoice, Guid>(litigationRequestInvoice);
                    }

                }
                #endregion

                #region Update processingLog
                processingLog.IsFinished = true;
                processingLog.FinishedOn = DateTime.Now;
                processingLog.ShowForApplicant = processingLog.WorkFlowStep.WorkFlowStepEvents.Single().ShowProcessingLogForApplicant;
                processingLog.WorkFlowStepEventId = requestPayload.WorkflowStepEventId;
                #endregion

                #region AllowChangeExternalStatus
                if (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().AllowChangeExternalStatus)
                {
                    responseWorkFlowDto.ExternalStatus = processingLog.WorkFlowStep.WorkFlowStepEvents.Single().ExternalStatusId;
                    litigationRequest.ExternalStatusId = processingLog.WorkFlowStep.WorkFlowStepEvents.Single().ExternalStatusId.Value;
                }
                #endregion

                #region AllowChangeInternalStatus
                if (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().AllowChangeInternalStatus)
                {
                    responseWorkFlowDto.InternalStatus = processingLog.WorkFlowStep.WorkFlowStepEvents.Single().InternalStatusId;
                    litigationRequest.InternalStatusId = processingLog.WorkFlowStep.WorkFlowStepEvents.Single().InternalStatusId.Value;

                }
                #endregion

                #region AllowChangeIsFinished
                if (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().AllowChangeIsFinished)
                    responseWorkFlowDto.IsFinished = processingLog.WorkFlowStep.WorkFlowStepEvents.Single().IsFinishedValue;
                #endregion

                #region AllowChangeIsActive
                if (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().AllowChangeIsActive)
                    responseWorkFlowDto.IsActive = processingLog.WorkFlowStep.WorkFlowStepEvents.Single().IsActiveValue;
                #endregion

                #region AllowNotes 
                if (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().AllowNotes)
                    processingLog.Notes = requestPayload.Notes;
                #endregion

                #region AllowFileUpload
                if (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().AllowFileUpload)
                {
                    requestPayload.UploadedFilesIds?.ForEach(fileId =>
                    {
                        processingLog.WorkFlowProcessingLogFiles.Add(new WorkFlowProcessingLogFile()
                        {
                            Id = Guid.NewGuid(),
                            FileId = fileId
                        });
                    });
                }
                #endregion

                #region AllowCompletionNotificationPlan 
                if (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().AllowCompletionNotificationPlan)
                {
                    if (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().CompletionNotificationPlanId.HasValue)
                        await ExecuteNotificationPlan(processingLog.WorkFlowStep.WorkFlowStepEvents.Single().CompletionNotificationPlanId.Value);
                }
                #endregion

                #region Create Log
                if (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().LogMessage != null)
                {
                    WorkFlowLogCreateUpdateDto workFlowLog = new WorkFlowLogCreateUpdateDto()
                    {
                        CreatedOn = DateTime.UtcNow,
                        ShowForApplicant = processingLog.WorkFlowStep.WorkFlowStepEvents.Single().ShowLogForApplicant,
                        LogMessage = processingLog.WorkFlowStep.WorkFlowStepEvents.Single().LogMessage + ((requestPayload.Notes != null) ? " - " + requestPayload.Notes : ""),
                        RequestId = requestPayload.RequestId
                    };
                    processingLog.WorkFlowStep.WorkFlowStepEvents.Single().Translations?.ToList().ForEach(x =>
                    {
                        workFlowLog.Translations.Add(new WorkFlowLogTranslation()
                        {
                            LanguageCode = x.LanguageCode,
                            LogMessage = x.LogMessage
                        });
                    });

                    var workFlowLogResponse = await _workFlowLogService.AddAsync(workFlowLog);
                    if (!workFlowLogResponse.IsDone)
                        return workFlowLogResponse;
                }
                #endregion

                #region EventActionMode
                if (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().ActionModeId.HasValue)
                {
                    switch (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().ActionModeId)
                    {
                        case EventActionMode.GoPreviousStep:
                            var previoudProcessingLog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())?
                                               .Where(x => x.Id == processingLog.SourceWorkFlowProcessingLogId)
                                               .Single();

                            if (previoudProcessingLog != null)
                            {
                                var newProcessingLog = new WorkFlowProcessingLog();
                                {
                                    newProcessingLog.CreatedOn = DateTime.Now;
                                    newProcessingLog.IsFinished = false;
                                    newProcessingLog.EmployeeId = previoudProcessingLog.EmployeeId;
                                    newProcessingLog.RequestId = previoudProcessingLog.RequestId;
                                    newProcessingLog.SourceWorkFlowProcessingLogId = processingLog.Id;
                                }
                                //get step id depend on workflow category type which order is equal 2 (reconciliation , arbitration)  
                                if (requestPayload.CategoryTypeChanged == true)
                                {
                                    var workflowId = (await _repository.GetQueryable<WorkFlow, Guid>())?
                                               .Include(x => x.WorkFlowSteps)
                                               .Where(x => x.WorkFlowName.ToLower() == requestPayload.CategoryType.ToLower())
                                               .Single();
                                    newProcessingLog.WorkFlowStepId = workflowId?.WorkFlowSteps.FirstOrDefault(s => s.Order == 2)?.Id ?? previoudProcessingLog.WorkFlowStepId;
                                }
                                else
                                {
                                    newProcessingLog.WorkFlowStepId = previoudProcessingLog.WorkFlowStepId;
                                }
                                var newProcessingLogResponse = await _workFlowProcessingLogService.AddAsync(newProcessingLog, cancellationToken);
                                if (!newProcessingLogResponse.IsDone)
                                    return newProcessingLogResponse;
                            }
                            break;
                        case EventActionMode.GoNextStep:
                            await ExecuteWorkFlowSteps(processingLog.WorkFlowStep.WorkFlowId,
                                                       requestPayload.RequestId,
                                                       processingLog.Id,
                                                       processingLog.WorkFlowStep.Order + 1,
                                                       processingLog.WorkFlowStep.WorkFlowStepEvents.Single().SameEmployee,
                                                       cancellationToken);
                            break;
                        case EventActionMode.SpecificStep:
                            if (processingLog.WorkFlowStep.WorkFlowStepEvents.Single().NextWorkFlowStepDestinationId.HasValue)
                            {
                                var specificworkFlowStep = (await _repository.GetQueryable<WorkFlowStep, Guid>())?
                                                           .Where(x => x.Id == processingLog.WorkFlowStep.WorkFlowStepEvents.Single().NextWorkFlowStepDestinationId)
                                                           .Include(x => x.Department)
                                                           .ThenInclude(x => x.SysEmployees
                                                           .OrderBy(x => x.WorkFlowProcessingLogs
                                                           .Count(x => !x.IsFinished))
                                                           .ThenBy(x => x.IsAdmin))
                                                           .ThenInclude(x => x.EmployeeVacationRequests)
                                                           .Single();

                                await ExecuteWorkFlowStep(specificworkFlowStep,
                                                          requestPayload.RequestId,
                                                          processingLog.Id,
                                                          processingLog.WorkFlowStep.WorkFlowStepEvents.Single().SameEmployee,
                                                          cancellationToken);
                            }
                            break;
                    }
                }
                #endregion

                var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
                var msg = "";
                string body;

                switch (litigationRequest.InternalStatusId)
                {

                    case RequestStatus.CollectionOfLitigationFeesRequestIsBeingSent:

                        litigationRequest.IsCollectionOfLitigationFeesRequestIsBeingSent = true;

                        // Send Notification
                        // تم إرسال طلب  دفع رسوم الدعوي رقم ....... لمقدم الطلب - موظف التدقيق - مدير ادارة التدقيق
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_CollectionOfLitigationFeesRequestIsBeingSent).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_CollectionOfLitigationFeesRequestIsBeingSent).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            SendNotificationMSPlaintiff(body, msg, litigationRequest, RequestType.LitigationRequest, "Collection Of Litigation Fees Request Is Being Sent");
                        }
                        catch (Exception e)
                        {

                        }
                        break;
                    case RequestStatus.LitigationFeesHaveBeenCollected:

                        litigationRequest.IsLitigationFeesHaveBeenCollected = true;

                        // Send Notification
                        // تم دفع رسوم الدعوي رقم ....... لمقدم الطلب - موظف التدقيق - مدير ادارة التدقيق
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_LitigationFeesHaveBeenCollected).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_LitigationFeesHaveBeenCollected).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            SendNotificationMSPlaintiff(body, msg, litigationRequest, RequestType.LitigationRequest, "Collection Of Litigation Fees Request Is Being Sent");
                        }
                        catch (Exception e)
                        {

                        }
                        break;
                    case RequestStatus.CollectionOfReconciliationFeesRequestIsBeingSent:

                        litigationRequest.IsCollectionOfReconciliationFeesRequestIsBeingSent = true;

                        // Send Notification
                        // تم إرسال طلب  دفع رسوم الصلح رقم ....... لمقدم الطلب - موظف التدقيق - مدير ادارة التدقيق
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_CollectionOfReconciliationFeesRequestIsBeingSent).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_CollectionOfReconciliationFeesRequestIsBeingSent).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            SendNotificationMSPlaintiff(body, msg, litigationRequest, RequestType.LitigationRequest, "Collection Of Reconciliation Fees Request Is Being Sent");
                        }
                        catch (Exception e)
                        {

                        }
                        break;

                    case RequestStatus.ReconciliationFeesHaveBeenCollected:

                        litigationRequest.IsReconciliationFeesHaveBeenCollected = true;

                        // Send Notification
                        // تم دفع رسوم الصلح رقم ....... لمقدم الطلب - موظف التدقيق - مدير ادارة التدقيق
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_ReconciliationFeesHaveBeenCollected).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_ReconciliationFeesHaveBeenCollected).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            SendNotificationMSPlaintiff(body, msg, litigationRequest, RequestType.LitigationRequest, "Reconciliation Fees Have Been Collected");
                        }
                        catch (Exception e)
                        {

                        }
                        break;

                    case RequestStatus.Rejected:
                        // Send Notification
                        // تم رفض الطلب
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Creation_Reject).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Creation_Reject).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            SendNotificationMSPlaintiff(body, msg, litigationRequest, RequestType.LitigationRequest, "Litigation Request Rejected");
                        }
                        catch (Exception e)
                        {

                        }



                        break;
                    case RequestStatus.SentBackForEdit:
                        // Send Notification
                        // تم إعادة الطلب لوجود بعض الملاحظات
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Creation_Resent).Result.Data?.Value;
                            int reminderForApplicantSecondPeriod = 0;
                            int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_SecondEditReminderForForRequestApplicant).Result.Data?.Value, out reminderForApplicantSecondPeriod);
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            msg = msg.Replace("{ExpirationDate}", $"{reminderForApplicantSecondPeriod / 24}");
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Creation_Resent).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                msg = msg.Replace("{ExpirationDate}", $"{reminderForApplicantSecondPeriod / 24}");
                            }
                            SendNotificationMSPlaintiff(body, msg, litigationRequest, RequestType.LitigationRequest, "Litigation Request Resent");
                        }
                        catch (Exception)
                        {

                        }


                        break;
                    case RequestStatus.Received:

                        switch (litigationRequest?.LitigationRequestCategoryId)
                        {
                            case REAC.Domain.Shared.Enum.LitigationRequestCategory.Arbitration:

                                litigationRequest.IsReconciliationFeesHaveBeenCollected = true;
                                // Send Notification
                                // تم دفع رسوم الصلح رقم ....... لمقدم الطلب - موظف التدقيق - مدير ادارة التدقيق

                                litigationRequest.LitigationRequestStage = LitigationRequestStage.Arbitration;
                                litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.ArbitratorSelectionDuration;
                                break;
                            default:
                                litigationRequest.LitigationRequestStage = LitigationRequestStage.SetReformer;
                                litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.SetReformerFirstTime;
                                break;
                        }

                        var response = _repository.Update<LitigationRequest, Guid>(litigationRequest);
                        if (!response.IsDone)
                            return response;

                        var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                        if (!repositorySaveResponse.IsDone)
                            return repositorySaveResponse;

                        switch (litigationRequest.LitigationRequestCategoryId)
                        {
                            case LitigationRequestCategory.Arbitration:
                                break;
                            default:
                                var department = (await _repository.GetQueryable<SysDepartment, Guid>())?
                                   .Where(x => x.DepartmentType == DepartmentType.Reconciliation)
                                   .SingleOrDefault();
                                if (department != null)
                                {
                                    switch (department.DistributionMode)
                                    {
                                        case DepartmentDistributionMode.ManualDistribution:

                                            // Send Notification
                                            // يوجد مهمه جديده برقم, برجاء تعيين مصلح عليها

                                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                            {
                                                Id = Guid.NewGuid(),
                                                CreatedOn = DateTime.Now,
                                                RequestId = litigationRequest.Id,
                                                ShowForApplicant = false,
                                                LogMessage = "تم اضافة الدعوي و بأنتظار تعيين مصلح من طرف مدير الصلح"
                                            });

                                            try
                                            {
                                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_NewRequest_ManualDistripution).Result.Data?.Value;
                                                msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);

                                                var lstNotification = new List<SysNotificationDto>();
                                                lstNotification.Add(new SysNotificationDto()
                                                {
                                                    ActorId = deptAdminInfoReconciliation.Data.Id,
                                                    CreatedOn = DateTime.Now,
                                                    ActorTypeId = NotificationActorType.Reformer,
                                                    IsViewed = false,
                                                    ViewedOn = null,
                                                    RequestId = litigationRequest.Id,
                                                    RequestTypeId = RequestType.ReformerTaskrequests,
                                                    Message = msg
                                                });


                                                await _notificationService.SendNotifications(new SendEmailDto()
                                                {
                                                    Body = msg,
                                                    Subject = "يوجد مهمه جديده , برجاء تعيين مصلح عليها",
                                                    To = new List<string>() { deptAdminInfoReconciliation.Data.Email }
                                                }, new SMSRequestDto()
                                                {
                                                    message_text = msg,
                                                    notification_type = "SMS",
                                                    recipients = new List<string>() { deptAdminInfoReconciliation.Data.Email }
                                                }, lstNotification);


                                            }
                                            catch (Exception)
                                            {

                                            }

                                            break;

                                        case DepartmentDistributionMode.AutoDistribution:
                                            var reformers = (await _repository.GetQueryable<Citizen, Guid>())?
                                                            .Include(x => x.ReformerLitigationRequests)
                                                            .Include(x => x.LitigationRequestReformerAssigningRequests)
                                                            .Where(x => x.IsReformer == true & x.ReformerStatusId == ReformerStatus.Certified
                                                            & x.Id != litigationRequest.ApplicantCitizenId
                                                            & x.Id != litigationRequest.Contract.FirstPartyCitizenId
                                                            & x.Id != litigationRequest.Contract.SecondPartyCitizenId)
                                                            .OrderBy(x => x.ReformerLitigationRequests
                                                            .Count(t => t.IsFinished == false) + x.LitigationRequestReformerAssigningRequests.Count(x => x.Response == LitigationRequestReformerAssigningRequestStatus.None)).ThenBy(x => x.ReformerLitigationRequests.Count)
                                                            .ToList();

                                            if (reformers != null)
                                            {
                                                if (reformers.Any())
                                                {
                                                    LitigationRequestReformerAssigningRequest litigationRequestReformerAssigningRequest = new LitigationRequestReformerAssigningRequest();
                                                    litigationRequestReformerAssigningRequest.CitizenId = reformers.FirstOrDefault().Id;
                                                    litigationRequestReformerAssigningRequest.RequestOn = DateTime.Now;
                                                    litigationRequestReformerAssigningRequest.LitigationRequestId = litigationRequest.Id;
                                                    litigationRequestReformerAssigningRequest.Response = LitigationRequestReformerAssigningRequestStatus.None;
                                                    await _repository.AddAsync<LitigationRequestReformerAssigningRequest, Guid>(litigationRequestReformerAssigningRequest);

                                                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        CreatedOn = DateTime.Now,
                                                        RequestId = litigationRequest.Id,
                                                        ShowForApplicant = false,
                                                        LogMessage = "تم إرسال طلب مهمة للمصلح " + reformers.FirstOrDefault()?.FullName
                                                    });
                                                }
                                                else
                                                {
                                                    return new BaseResponse()
                                                    {
                                                        IsDone = false,
                                                        Message = "لا يوجد مصلحين لتولي المهمه برجاء الرجوع للمركز"
                                                    };
                                                }
                                                var approveSaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                                                if (!approveSaveResponse.IsDone)
                                                    return approveSaveResponse;

                                                // Send Notification
                                                // يوجد طلب مهمه جديدة كمصلح علي الدعوي رقم
                                                try
                                                {
                                                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_NewRequest).Result.Data?.Value;
                                                    msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                    using (StreamReader streamReader = File.OpenText(pathToFile))
                                                    {
                                                        body = await streamReader.ReadToEndAsync();
                                                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_NewRequest).Result.Data?.Value);
                                                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                    }

                                                    var lstNotification = new List<SysNotificationDto>();
                                                    lstNotification.Add(new SysNotificationDto()
                                                    {
                                                        ActorId = reformers.FirstOrDefault().Id,
                                                        CreatedOn = DateTime.Now,
                                                        ActorTypeId = NotificationActorType.Reformer,
                                                        IsViewed = false,
                                                        ViewedOn = null,
                                                        RequestId = litigationRequest.Id,
                                                        RequestTypeId = RequestType.ReformerTaskrequests,
                                                        Message = msg
                                                    });


                                                    await _notificationService.SendNotifications(new SendEmailDto()
                                                    {
                                                        Body = body,
                                                        Subject = "يوجد طلب مهمه جديدة كمصلح",
                                                        To = new List<string>() { reformers.FirstOrDefault()?.Email }
                                                    }, new SMSRequestDto()
                                                    {
                                                        message_text = msg,
                                                        notification_type = "SMS",
                                                        recipients = new List<string>() { reformers.FirstOrDefault()?.Mobile }
                                                    }, lstNotification);


                                                }
                                                catch (Exception)
                                                {

                                                }

                                                // Send Notification
                                                // يوجد طلب مهمه جديدة كمصلح علي الدعوي رقم
                                                try
                                                {
                                                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_ReSendTask).Result.Data?.Value;
                                                    msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                                    msg = msg.Replace("{ReconciliatorName}", reformers.FirstOrDefault()?.FullName);
                                                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                    using (StreamReader streamReader = File.OpenText(pathToFile))
                                                    {
                                                        body = await streamReader.ReadToEndAsync();
                                                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_ReSendTask).Result.Data?.Value);
                                                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                        body = body?.Replace("{ReconciliatorName}", reformers.FirstOrDefault()?.FullName);
                                                    }

                                                    var lstNotification = new List<SysNotificationDto>();
                                                    lstNotification.Add(new SysNotificationDto()
                                                    {
                                                        ActorId = deptAdminInfoReconciliation.Data.Id,
                                                        CreatedOn = DateTime.Now,
                                                        ActorTypeId = NotificationActorType.Reformer,
                                                        IsViewed = false,
                                                        ViewedOn = null,
                                                        RequestId = litigationRequest.Id,
                                                        RequestTypeId = RequestType.ReformerTaskrequests,
                                                        Message = msg
                                                    });


                                                    await _notificationService.SendNotifications(new SendEmailDto()
                                                    {
                                                        Body = body,
                                                        Subject = "يوجد طلب مهمه جديدة لمصلح",
                                                        To = new List<string>() { deptAdminInfoReconciliation.Data.Email }
                                                    }, new SMSRequestDto()
                                                    {
                                                        message_text = msg,
                                                        notification_type = "SMS",
                                                        recipients = new List<string>() { deptAdminInfoReconciliation.Data.Mobile }
                                                    }, lstNotification);


                                                }
                                                catch (Exception)
                                                {

                                                }



                                            }
                                            break;
                                    }
                                }
                                break;
                        }




                        // Send Notification
                        // تم قبول الدعوى واستلامها
                        try
                        {
                            var portalUrl = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.REACPortalUrl).Result.Data?.Value;

                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Creation_Accept).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            msg = msg?.Replace("{PortalUrl}", portalUrl);

                            string msgDefendant = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Creation_Accept_Defendant).Result.Data?.Value;
                            msgDefendant = msgDefendant.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            msgDefendant = msgDefendant.Replace("{PortalUrl}", portalUrl);
                            var bodyDefendant = "";
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Creation_Accept).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);

                            }

                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                bodyDefendant = await streamReader.ReadToEndAsync();
                                bodyDefendant = bodyDefendant?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Creation_Accept_Defendant).Result.Data?.Value);
                                bodyDefendant = bodyDefendant?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                bodyDefendant = bodyDefendant.Replace("{PortalUrl}", portalUrl);

                            }

                            await SendNotificationMS(body, msg, msg, litigationRequest, RequestType.LitigationRequest, "Litigation Request Accepted", true);
                            await SendNotificationForDefendant(bodyDefendant, msgDefendant, litigationRequest, RequestType.LitigationRequest, "Litigation Request Accepted", true);
                        }
                        catch (Exception)
                        {

                        }


                        break;
                    case RequestStatus.Edited:
                        // Send Notification
                        // تم تعديل الدعوى وإرسالها مرة أخرى
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ProcessWorkflow_Edited).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ProcessWorkflow_Edited).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            SendNotificationMSPlaintiff(body, msg, litigationRequest, RequestType.LitigationRequest, "Litigation Request Updated and Resent");
                        }
                        catch (Exception e)
                        {

                        }

                        break;
                    case RequestStatus.AgreementRequestHasBeenSent:
                        if (litigationRequest.IsAgreementRequestRequired.HasValue)
                            return new BaseResponse() { IsDone = false, Message = "تم طلب المشارطة من قبل" };

                        litigationRequest.IsAgreementRequestRequired = true;
                        litigationRequest.AgreementRequestRequiredOn = DateTime.Now;
                        litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAgreementRequestRequired;
                        var repositorySaveRes = await _repository.SaveChangesAsync(new CancellationToken());
                        if (!repositorySaveRes.IsDone)
                            return repositorySaveRes;

                        // Send Notification
                        // تم إرسال طلب المشارطة
                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.WorkFlow_Condition).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            string msgReview = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.WorkFlow_Condition_Review).Result.Data?.Value;
                            msgReview = msgReview.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.WorkFlow_Condition_Review).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            SendNotificationMS(body, msg, msgReview, litigationRequest, RequestType.LitigationAgreementRequest, "Litigation Request Resent");
                        }
                        catch (Exception e)
                        {

                        }

                        break;
                }




            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
            return new DataResponse<ResponseWorkFlowDto>()
            {
                IsDone = true,
                Data = responseWorkFlowDto
            };
        }
        //المدعى -مدير ادارة التدقيق
        private async Task SendNotificationMSPlaintiff(string mailMsg, string msg, LitigationRequest litigationRequest, RequestType requestTypeId, string subject)
        {
            var deptAdminInfoReview = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);
            var employee = litigationRequest.WorkFlowProcessingLogs.Where(w => w.EmployeeId != null).OrderBy(x => x.CreatedOn).LastOrDefault()?.Employee;

            //ApplicantCitizen
            var objEmail = new SendEmailDto()
            {
                Body = mailMsg,
                Subject = subject,
                To = new List<string>() { litigationRequest.ApplicantCitizen?.Email }

            };
            var objSms = new SMSRequestDto()
            {
                message_text = msg,
                recipients = new List<string>() { litigationRequest.ApplicantCitizen.Mobile },
                notification_type = "SMS",
            };
            var lstNotification = new List<SysNotificationDto>();
            lstNotification.Add(new SysNotificationDto()
            {
                ActorId = litigationRequest.ApplicantCitizen.Id,
                CreatedOn = DateTime.Now,
                ActorTypeId = NotificationActorType.Plaintiff,
                IsViewed = false,
                ViewedOn = null,
                RequestId = litigationRequest.Id,
                RequestTypeId = requestTypeId,
                Message = msg,
            });

            //مدير ادارة التدقيق
            if (deptAdminInfoReview is not null && deptAdminInfoReview.Data is not null)
            {
                objEmail.To.Add(deptAdminInfoReview?.Data?.Email);
                objSms.recipients.Add(deptAdminInfoReview?.Data?.Mobile);

                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = deptAdminInfoReview.Data.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Employee,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });
            }

            // الموظف المعين علي الدعوي
            if (employee is not null)
            {
                objEmail.To.Add(employee?.Email);
                objSms.recipients.Add(employee?.Mobile);

                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = employee.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Employee,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });
            }


            if (litigationRequest?.Contract?.FirstPartyCitizen != null && litigationRequest?.Contract?.FirstPartyCitizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest?.Contract?.FirstPartyCitizen.Email);
                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.FirstPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });
                objSms.recipients.Add(litigationRequest?.Contract?.FirstPartyCitizen.Mobile);


            }
            //if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility?.Citizen?.Id != litigationRequest.ApplicantCitizen.Id)
            //{
            //    objEmail.To.Add(litigationRequest?.Contract?.FirstPartyFacility.Email);
            //    lstNotification.Add(new SysNotificationDto()
            //    {
            //        ActorId = (Guid)litigationRequest?.Contract?.FirstPartyFacility?.Citizen.Id,
            //        CreatedOn = DateTime.Now,
            //        ActorTypeId = NotificationActorType.Plaintiff,
            //        IsViewed = false,
            //        ViewedOn = null,
            //        RequestId = litigationRequest.Id,
            //        RequestTypeId = requestTypeId,
            //        Message = msg,
            //    });
            //    objSms.recipients.Add(litigationRequest?.Contract?.FirstPartyFacility?.Citizen?.Mobile);


            //}
            await _notificationService.SendNotifications(objEmail, objSms, lstNotification);

        }
        //المدعى -مدير ادارة التدقيق -المدعى عليه
        private async Task SendNotificationMS(string mailMsg, string msg, string msgReview, LitigationRequest litigationRequest, RequestType requestTypeId, string subject, bool? sendToArbitratorAdmin = null)
        {
            var deptAdminInfoReview = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);
            var deptAdminInfoArbitrator = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
            //ApplicantCitizen
            var objEmail = new SendEmailDto()
            {
                Body = mailMsg,
                Subject = subject,
                To = new List<string>() { litigationRequest.ApplicantCitizen?.Email }

            };
            var objSms = new SMSRequestDto()
            {
                message_text = msg,
                recipients = new List<string>() { litigationRequest.ApplicantCitizen.Mobile },
                notification_type = "SMS",
            };
            var lstNotification = new List<SysNotificationDto>();
            lstNotification.Add(new SysNotificationDto()
            {
                ActorId = litigationRequest.ApplicantCitizen.Id,
                CreatedOn = DateTime.Now,
                ActorTypeId = NotificationActorType.Plaintiff,
                IsViewed = false,
                ViewedOn = null,
                RequestId = litigationRequest.Id,
                RequestTypeId = requestTypeId,
                Message = msg,
            });

            //مدير ادارة التدقيق
            if (deptAdminInfoReview is not null)
            {
                objEmail.To.Add(deptAdminInfoReview?.Data?.Email);
                objSms.recipients.Add(deptAdminInfoReview?.Data?.Mobile);

                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = deptAdminInfoReview.Data.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Employee,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msgReview,
                });
            }

            //مدير ادارة التحكيم
            if (deptAdminInfoArbitrator is not null & sendToArbitratorAdmin is not null)
            {
                if ((bool)sendToArbitratorAdmin)
                {
                    objEmail.To.Add(deptAdminInfoArbitrator?.Data?.Email);
                    objSms.recipients.Add(deptAdminInfoArbitrator?.Data?.Mobile);

                    lstNotification.Add(new SysNotificationDto()
                    {
                        ActorId = deptAdminInfoArbitrator.Data.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Employee,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = litigationRequest.Id,
                        RequestTypeId = requestTypeId,
                        Message = msgReview,
                    });
                }

            }
            //المدعين
            if (litigationRequest?.Contract?.FirstPartyCitizen != null && litigationRequest?.Contract?.FirstPartyCitizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest?.Contract?.FirstPartyCitizen.Email);
                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.FirstPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });
                objSms.recipients.Add(litigationRequest?.Contract?.FirstPartyCitizen.Mobile);


            }
            //if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility?.Citizen?.Id != litigationRequest.ApplicantCitizen.Id)
            //{
            //    objEmail.To.Add(litigationRequest?.Contract?.FirstPartyFacility.Email);
            //    lstNotification.Add(new SysNotificationDto()
            //    {
            //        ActorId = (Guid)litigationRequest?.Contract?.FirstPartyFacility?.Citizen.Id,
            //        CreatedOn = DateTime.Now,
            //        ActorTypeId = NotificationActorType.Plaintiff,
            //        IsViewed = false,
            //        ViewedOn = null,
            //        RequestId = litigationRequest.Id,
            //        RequestTypeId = requestTypeId,
            //        Message = msg,
            //    });
            //    objSms.recipients.Add(litigationRequest?.Contract?.FirstPartyFacility?.Citizen?.Mobile);


            //}

            await _notificationService.SendNotifications(objEmail, objSms, lstNotification);

        }

        private async Task SendNotificationForDefendant(string mailMsg, string msg, LitigationRequest litigationRequest, RequestType requestTypeId, string subject, bool? sendToArbitratorAdmin = null)
        {
            // defendant
            if (litigationRequest?.Contract?.SecondPartyCitizen != null)
            {
                var objEmail = new SendEmailDto()
                {
                    Body = mailMsg,
                    Subject = subject,
                    To = new List<string>() { litigationRequest.Contract.SecondPartyCitizen?.Email }

                };
                var objSms = new SMSRequestDto()
                {
                    message_text = msg,
                    recipients = new List<string>() { litigationRequest.Contract.SecondPartyCitizen?.Mobile },
                    notification_type = "SMS",
                };

                var lstNotification = new List<SysNotificationDto>();

                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.SecondPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Defendant,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });

                await _notificationService.SendNotifications(objEmail, objSms, lstNotification);
            }


        }

        #region Invoice
        public async Task<DataResponse<InvoiceDataDto>> CreateIntegratedInvoice(LitigationRequest litigation, string fees,string serviceId)
        {
            var accessToken = (await _gEELoginService.Login()).Data?.Access_token;
            try
            {
                var apiUrl = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.CreateInvoice_URL).Result.Data?.Value;
                //var Administrative_ServiceId = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Fees_Billing).Result.Data?.Value;

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
                    Type = "2",
                    IdentityNumber = litigation.ApplicantCitizen?.IdentityNumber,
                    MobileNumber = litigation.ApplicantCitizen?.Mobile,
                    Email = litigation.ApplicantCitizen?.Email,
                    CaseNumber = serviceId == "1970" ? "RCaseNu10XXX1": "LCaseNu10XXX1",
                    CaseDescription = serviceId == "1970" ?"Reconciliation Fees":"Litigation Fees",
                    RefIdPltfrm = serviceId == "1970"? "R-"+litigation.RequestCode:"L-" + litigation.RequestCode
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
    }
}
