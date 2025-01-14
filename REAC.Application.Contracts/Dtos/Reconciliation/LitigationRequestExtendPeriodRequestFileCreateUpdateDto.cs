using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestExtendPeriodRequestFileCreateUpdateDto : BaseEntity<Guid>
{
    public Guid LitigationRequestExtendPeriodRequestId { get; set; }

    public virtual SysFileManagerCreateUpdateDto FileManager { get; set; } = null!;

}
