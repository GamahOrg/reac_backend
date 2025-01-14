using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Reconciliation.Process
{
    public class UpdateLitigationRequestForReportPayload
    {
        public Guid RequestId { get; set; }
        public ApproveRejectStatus MinutesOfMeetingApprovalStatus { get; set; }
    }
}
