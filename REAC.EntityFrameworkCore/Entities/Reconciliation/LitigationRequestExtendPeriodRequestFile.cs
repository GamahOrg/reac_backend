using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestExtendPeriodRequestFile : BaseEntity<Guid>
{
    public Guid LitigationRequestExtendPeriodRequestId { get; set; }

    public Guid FileManagerId { get; set; }

    public virtual SysFileManager FileManager { get; set; } = null!;

    public virtual LitigationRequestExtendPeriodRequest LitigationRequestExtendPeriodRequest { get; set; } = null!;
}
