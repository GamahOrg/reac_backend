using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestReconciliationCenterDegreeFile : BaseEntity<Guid>
{
    public Guid CitizenRequestId { get; set; }

    public Guid ReconciliationCenterDegreeFileId { get; set; }

    public virtual SysFileManager? ReconciliationCenterFile { get; set; }

    public virtual CitizenRequest? CitizenRequest { get; set; }
}

