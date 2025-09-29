using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestCorrespondenceFile : AuditedBaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request correspondence")]
    public Guid LitigationRequestCorrespondenceId { get; set; }

    [Comment("Unique identifier of the litigation request correspondence")]
    public Guid FileManagerId { get; set; }

    public virtual SysFileManager FileManager { get; set; } = null!;

    public virtual LitigationRequestCorrespondence LitigationRequestCorrespondence { get; set; } = null!;
}

