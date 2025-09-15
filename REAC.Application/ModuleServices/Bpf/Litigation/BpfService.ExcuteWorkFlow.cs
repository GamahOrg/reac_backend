
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Wpf.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace Bpf.Application.ModuleServices
{
    public partial class BpfService
    {
        public async Task<BaseResponse> ExcuteWorkFlow(Guid requestId, CancellationToken cancellationToken, int? orderId = null, bool? sameEmployee = null)
        {
            try
            {
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                    .Include(x => x.Delegator.Citizen)
                    .Include(x => x.WorkFlowProcessingLogs)
                    .ThenInclude(x => x.Employee)
                            .Include(x => x.ApplicantCitizen)
                          .Include(x => x.LitigationRequestCategory)
                          .Include(x => x.Contract)
                         .ThenInclude(x => x.FirstPartyCitizen)
                         .Include(x => x.Contract)
                         .ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                          .AsNoTracking()
                          .FirstOrDefault(x => x.Id == requestId);

                BaseResponse response;

                if (orderId is not null)
                    response = await ExecuteWorkFlowSteps(litigationRequest.LitigationRequestCategory.BpfId, requestId, null, (int)orderId, sameEmployee, cancellationToken);
                else
                    response = await ExecuteWorkFlowSteps(litigationRequest.LitigationRequestCategory.BpfId, requestId, null, 1, sameEmployee, cancellationToken);



                litigationRequest.InternalStatusId = (response as DataResponse<ResponseWorkFlowDto>).Data.InternalStatus is not null ? (response as DataResponse<ResponseWorkFlowDto>).Data.InternalStatus.Value : RequestStatus.New;
                litigationRequest.ExternalStatusId = (response as DataResponse<ResponseWorkFlowDto>).Data.ExternalStatus is not null ? (response as DataResponse<ResponseWorkFlowDto>).Data.ExternalStatus.Value : RequestStatus.New;
                litigationRequest.IsSent = true;
                litigationRequest.SentOn = DateTime.Now;
                litigationRequest.IsActive = (response as DataResponse<ResponseWorkFlowDto>).Data.IsActive;
                litigationRequest.IsFinished = (response as DataResponse<ResponseWorkFlowDto>).Data.IsFinished;
                litigationRequest.IsAllowEdit = false;

                var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                if (!repositorySaveResponse.IsDone)
                    return repositorySaveResponse;
                try
                {
                    // Send Notification
                    // تم إرسال الدعوى

                    string msg = "";
                    string body;
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ExcuteWorkflow).Result.Data?.Value;
                    msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    using (StreamReader streamReader = File.OpenText(pathToFile))
                    {
                        body = await streamReader.ReadToEndAsync();
                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ExcuteWorkflow).Result.Data?.Value);
                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                    }

                    litigationRequest.WorkFlowProcessingLogs = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())
                    .Include(x => x.Employee).ToList();

                    await SendNotificationMSPlaintiff(body, msg, litigationRequest, RequestType.LitigationRequest, "Litigation Request Sent");
                }
                catch (Exception e)
                {

                }

                // Notify first party if there is delegator
                if (litigationRequest.DelegatorId is not null && litigationRequest.Delegator.Citizen is not null)
                {

                    string msg = "";
                    string body;
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ExcuteWorkflow_DelegatorFirstParty).Result.Data?.Value;
                    msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    using (StreamReader streamReader = File.OpenText(pathToFile))
                    {
                        body = await streamReader.ReadToEndAsync();
                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ExcuteWorkflow).Result.Data?.Value);
                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                    }


                    var objEmail = new SendEmailDto()
                    {
                        Body = body,
                        Subject = "Litigation Request Sent",
                        To = new List<string>() { litigationRequest.Delegator?.Citizen?.Email }

                    };
                    var objSms = new SMSRequestDto()
                    {
                        message_text = msg,
                        recipients = new List<string>() { litigationRequest.Delegator?.Citizen?.Mobile },
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
                        RequestTypeId = RequestType.LitigationRequest,
                        Message = msg,
                    });

                    var firstPartyNotify = await _notificationService.SendNotifications(objEmail, objSms, lstNotification);


                }

                return response;
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString(),
                };
            }
        }

        public async Task<BaseResponse> ExecuteWorkFlowForSLA(Guid requestId, CancellationToken cancellationToken, int? orderId = null)
        {
            try
            {
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                    .Include(x => x.WorkFlowProcessingLogs)
                    .ThenInclude(x => x.Employee)
                            .Include(x => x.ApplicantCitizen)
                          .Include(x => x.LitigationRequestCategory)
                          .Include(x => x.Contract)
                         .ThenInclude(x => x.FirstPartyCitizen)
                         .Include(x => x.Contract)
                         .ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                          .AsNoTracking()
                          .Where(x => x.Id == requestId).ToList().FirstOrDefault();

                BaseResponse response;

                if (orderId is not null)
                    response = await ExecuteWorkFlowStepsForSLA(litigationRequest.LitigationRequestCategory.BpfId, requestId, null, (int)orderId, null, cancellationToken);
                else
                    response = await ExecuteWorkFlowStepsForSLA(litigationRequest.LitigationRequestCategory.BpfId, requestId, null, 1, null, cancellationToken);



                litigationRequest.InternalStatusId = (response as DataResponse<ResponseWorkFlowDto>).Data.InternalStatus is not null ? (response as DataResponse<ResponseWorkFlowDto>).Data.InternalStatus.Value : RequestStatus.New;
                litigationRequest.ExternalStatusId = (response as DataResponse<ResponseWorkFlowDto>).Data.ExternalStatus is not null ? (response as DataResponse<ResponseWorkFlowDto>).Data.ExternalStatus.Value : RequestStatus.New;
                litigationRequest.IsSent = true;
                litigationRequest.SentOn = DateTime.Now;
                litigationRequest.IsActive = (response as DataResponse<ResponseWorkFlowDto>).Data.IsActive;
                litigationRequest.IsFinished = (response as DataResponse<ResponseWorkFlowDto>).Data.IsFinished;
                litigationRequest.IsAllowEdit = false;

                var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                if (!repositorySaveResponse.IsDone)
                    return repositorySaveResponse;


                return response;
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString(),
                };
            }
        }
    }
}
