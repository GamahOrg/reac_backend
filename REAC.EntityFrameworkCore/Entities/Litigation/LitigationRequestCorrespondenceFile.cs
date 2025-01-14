using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestCorrespondenceFile : AuditedBaseEntity<Guid>
{
    public Guid LitigationRequestCorrespondenceId { get; set; }

    public Guid FileManagerId { get; set; }

    public virtual SysFileManager FileManager { get; set; } = null!;

    public virtual LitigationRequestCorrespondence LitigationRequestCorrespondence { get; set; } = null!;
}
