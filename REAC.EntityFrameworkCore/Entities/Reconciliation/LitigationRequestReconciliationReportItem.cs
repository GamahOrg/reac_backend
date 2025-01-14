using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestReconciliationReportItem : BaseEntity<Guid>
{
    public Guid LitigationRequestReconciliationReportId { get; set; }

    public string ReconciliationItem { get; set; } = null!;

    public virtual LitigationRequestReconciliationReport LitigationRequestReconciliationReport { get; set; } = null!;
}
