using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestCVFile : BaseEntity<Guid>
{
    [Comment("Unique identifier of the citizen request")]
    public Guid CitizenRequestId { get; set; }

    [Comment("Unique identifier of the CV file")]
    public Guid CVFileId { get; set; }

    public virtual SysFileManager? CitizenRequestCVFiles { get; set; }

    public virtual CitizenRequest? CitizenRequest { get; set; }
}


