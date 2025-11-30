using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestReconciliationReport : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request")]
    public Guid LitigationRequestId { get; set; }

    [Comment("Date when the reconciliation report was created")]
    public DateTime ReportDate { get; set; }

    [Comment("Description for Reconciliation Session Details")]
    public string ReconciliationSessionDetails { get; set; } = null!;

    [Comment("Description for Modification Request")]
    public string? ModificationRequest { get; set; } = null!;

    public int? ReconciliationRequestDecision { get; set; }
    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual ICollection<LitigationRequestReconciliationReportItem> LitigationRequestMeetingReconciliationItems { get; set; } = new List<LitigationRequestReconciliationReportItem>();

}

