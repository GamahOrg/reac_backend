using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public class LitigationRequestExtendPeriodRequestCreateUpdateDto : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public int RequiredDays { get; set; }

    public string RequestReason { get; set; } = null!;

    public List<Guid> Files { get; set; }
}


