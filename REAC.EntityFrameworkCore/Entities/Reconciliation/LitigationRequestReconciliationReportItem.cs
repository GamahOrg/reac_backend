using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestReconciliationReportItem : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request reconciliation report")]
    public Guid LitigationRequestReconciliationReportId { get; set; }

    [Comment("Description for Reconciliation Item")]
    public string ReconciliationItem { get; set; } = null!;

    public virtual LitigationRequestReconciliationReport LitigationRequestReconciliationReport { get; set; } = null!;
}

