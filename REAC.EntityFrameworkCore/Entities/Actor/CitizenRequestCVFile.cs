using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestCVFile : BaseEntity<Guid>
{
    public Guid CitizenRequestId { get; set; }

    public Guid CVFileId { get; set; }

    public virtual SysFileManager? CitizenRequestCVFiles { get; set; }

    public virtual CitizenRequest? CitizenRequest { get; set; }
}

