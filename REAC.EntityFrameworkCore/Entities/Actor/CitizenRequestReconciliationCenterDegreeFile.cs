using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestReconciliationCenterDegreeFile : BaseEntity<Guid>
{
    [Comment("Unique identifier of the citizen request")]
    public Guid CitizenRequestId { get; set; }

    [Comment("Unique identifier of the reconciliation center degree file")]
    public Guid ReconciliationCenterDegreeFileId { get; set; }

    public virtual SysFileManager? ReconciliationCenterFile { get; set; }

    public virtual CitizenRequest? CitizenRequest { get; set; }
}


