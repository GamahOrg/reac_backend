using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Reconciliation;



namespace REAC.Application.Contracts.IMobileAppServices.Reconciliation
{
    public interface IMobileReconciliationService
    {

        #region Citizen Report Reply
        Task<BaseResponse> GetCitizenLitigationRequestForReportReply(int pageIndex, int pageSize);
        Task<BaseResponse> GetCitizenLitigationRequestForReportReply(Guid requestId);
        Task<BaseResponse> AddCitizenLitigationRequestForReportReply(Guid requestId, ApproveRejectStatus requestMinutesOfMeetingApprovalStatus);
        #endregion

    }
}