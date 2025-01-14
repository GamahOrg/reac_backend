using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Application.Contracts.Dtos.Reconciliation.Process
{
    public class AgreementRequestPayload
    {
        public Guid RequestId { get; set; }
        public ApproveRejectStatus agreementRequestStatus { get; set; }

    }
    public class WritingMinutesOfTheMeetingRequestPayload
    {
        public Guid RequestId { get; set; }
        public string ReconciliationSessionDetails { get; set; } = null!;
        public ReconciliationRequestDecision ReconciliationRequestDecisionStatus { get; set; }
        public string? ModificationRequest { get; set; } = null!;
        public List<Guid>? UploadedFilesIds { get; set; }
        public virtual ICollection<LitigationRequestReconciliationReportItemCreateUpdateDto> LitigationRequestMeetingReconciliationItems { get; set; } = new List<LitigationRequestReconciliationReportItemCreateUpdateDto>();
    }
    public class WritingArbitrationReportRequestPayload
    {
        public Guid Id { get; set; }
        public string Facts { get; set; } = null!;
        public string Reasons { get; set; } = null!;
        public string JudgmentDecisionText { get; set; } = null!;
        public string ModificationRequest { get; set; } = null!;
        public DateTime JudgmentDecisionDate { get; set; }
        public string JudgmentDecisionPlace { get; set; } = null!;
        public Guid MeetingLocationId { get; set; }
    }
}
