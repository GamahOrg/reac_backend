using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestExtendPeriodRequestFile : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request extend period request")]
    public Guid LitigationRequestExtendPeriodRequestId { get; set; }

    [Comment("Unique identifier of the file manager")]
    public Guid FileManagerId { get; set; }

    public virtual SysFileManager FileManager { get; set; } = null!;

    public virtual LitigationRequestExtendPeriodRequest LitigationRequestExtendPeriodRequest { get; set; } = null!;
}

