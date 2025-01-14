
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Wpf.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace Bpf.Application.ModuleServices
{
    public partial class BpfService
    {
        public async Task<BaseResponse> ExcuteWorkFlow(Guid requestId, CancellationToken cancellationToken)
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


                var response = await ExecuteWorkFlowSteps(litigationRequest.LitigationRequestCategory.BpfId, requestId, null, 1, null, cancellationToken);


                litigationRequest.InternalStatusId = (response as DataResponse<ResponseWorkFlowDto>).Data.InternalStatus.Value;
                litigationRequest.ExternalStatusId = (response as DataResponse<ResponseWorkFlowDto>).Data.ExternalStatus.Value;
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
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ExcuteWorkflow).Result.Data?.Value;
                    msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);

                    litigationRequest.WorkFlowProcessingLogs = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())
                    .Include(x => x.Employee).ToList();

                    SendNotificationMSPlaintiff(msg, litigationRequest, RequestType.LitigationRequest, "Litigation Request Sent");
                }
                catch (Exception e)
                {

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
    }
}
