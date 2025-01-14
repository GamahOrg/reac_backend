using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestReconciliationReport : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public DateTime ReportDate { get; set; }

    public string ReconciliationSessionDetails { get; set; } = null!;

    public string? ModificationRequest { get; set; } = null!;
    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual ICollection<LitigationRequestReconciliationReportItem> LitigationRequestMeetingReconciliationItems { get; set; } = new List<LitigationRequestReconciliationReportItem>();

}
