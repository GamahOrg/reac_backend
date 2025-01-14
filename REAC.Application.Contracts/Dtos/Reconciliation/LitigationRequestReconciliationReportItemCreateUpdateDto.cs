using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestReconciliationReportItemCreateUpdateDto : BaseEntity<Guid>
{
    public string ReconciliationItem { get; set; } = null!;

}
