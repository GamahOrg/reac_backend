using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Reconciliation;



namespace REAC.Application.ModuleServices
{
    public interface IReconciliationService
    {

        Task<BaseResponse> AddLitigationRequestDecision(LitigationRequestAction requestPayload, CancellationToken cancellationToken);

        Task<BaseResponse> GetLitigationRequestForSession(CancellationToken cancellationToken);

        Task<BaseResponse> AddReconciliationSession(ScheduleSessionProcessRequestPayload requestPayload, CancellationToken cancellationToken);

        Task<BaseResponse> GetLitigationRequestForSession(Guid requestId, CancellationToken cancellationToken);

        Task<BaseResponse> AddReconciliationReport(WritingMinutesOfTheMeetingRequestPayload requestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> Assign(AssignRequestPayload assignRequestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForFullAllReformer(int pageIndex, int pageSize, CancellationToken cancellationToken);

        Task<BaseResponse> GetLitigationRequestForAllSessionForReformer(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForFullAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken);

        Task<BaseResponse> GetLitigationRequestForReport(Guid requestId, CancellationToken cancellationToken);

        Task<BaseResponse> GetLitigationRequestForReport(CancellationToken cancellationToken);

        #region Citizen Report Reply
        Task<BaseResponse> GetCitizenLitigationRequestForReportReply(CancellationToken cancellationToken);
        Task<BaseResponse> GetCitizenLitigationRequestForReportReply(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> AddCitizenLitigationRequestForReportReply(Guid requestId, ApproveRejectStatus requestMinutesOfMeetingApprovalStatus, CancellationToken cancellationToken);
        #endregion


        #region Extend Period Request
        Task<BaseResponse> AddReconciliationExtendPeriod(LitigationRequestExtendPeriodRequestCreateUpdateDto requestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestExtendPeriod(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestExtendPeriod(CancellationToken cancellationToken);
        Task<BaseResponse> AddReconciliationExtendPeriodForEmployee(LitigationRequestExtendPeriodRequestCreateUpdateEmployeeDto requestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestExtendPeriodForEmployee(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestExtendPeriodForEmployee(int pageIndex, int pageSize, CancellationToken cancellationToken);
        #endregion

        Task<BaseResponse> CallNafathForReconciliationReport(Guid requestId, ApproveRejectStatus agreementRequestStatus);
    }
}