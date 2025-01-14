using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestRequestFile : BaseEntity<Guid>
{
    public Guid LitigationRequestRequestId { get; set; }

    public Guid FileManagerId { get; set; }

    public virtual SysFileManager FileManager { get; set; } = null!;

    public virtual LitigationRequestRequest LitigationRequestRequest { get; set; } = null!;
}
