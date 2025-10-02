using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestGroundFile : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request ground")]
    public Guid LitigationRequestGroundId { get; set; }

    [Comment("Unique identifier of the litigation request ground")]
    public Guid FileManagerId { get; set; }

    public virtual SysFileManager FileManager { get; set; } = null!;

    public virtual LitigationRequestGround LitigationRequestGround { get; set; } = null!;
}

