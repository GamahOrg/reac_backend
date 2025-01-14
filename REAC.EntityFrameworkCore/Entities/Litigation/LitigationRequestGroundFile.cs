using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestGroundFile : BaseEntity<Guid>
{
    public Guid LitigationRequestGroundId { get; set; }

    public Guid FileManagerId { get; set; }

    public virtual SysFileManager FileManager { get; set; } = null!;

    public virtual LitigationRequestGround LitigationRequestGround { get; set; } = null!;
}
