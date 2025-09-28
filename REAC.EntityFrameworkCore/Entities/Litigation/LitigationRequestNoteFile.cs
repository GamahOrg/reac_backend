using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestNoteFile : AuditedBaseEntity<Guid>
{
    [Comment("Unique identifier of the note")]
    public Guid NoteId { get; set; }

    [Comment("Unique identifier of the note")]
    public Guid FileManagerId { get; set; }

    public virtual SysFileManager FileManager { get; set; } = null!;

    public virtual LitigationRequestNote Note { get; set; } = null!;
}

