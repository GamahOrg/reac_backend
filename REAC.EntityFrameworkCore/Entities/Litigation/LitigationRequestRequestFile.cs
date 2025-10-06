using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestRequestFile : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request request")]
    public Guid LitigationRequestRequestId { get; set; }

    [Comment("Unique identifier of the litigation request request")]
    public Guid FileManagerId { get; set; }

    public virtual SysFileManager FileManager { get; set; } = null!;

    public virtual LitigationRequestRequest LitigationRequestRequest { get; set; } = null!;
}

