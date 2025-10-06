using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestNote : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request")]
    public Guid LitigationRequestId { get; set; }

    [Comment("Title of the litigation request note")]
    public string? Title { get; set; }

    [Comment("Detailed notes or comments about the litigation request")]
    public string Notes { get; set; } = null!;

    [Comment("Unique identifier of the user who created this note")]
    public Guid? CreatedBy { get; set; }
    
    [Comment("Date and time when the record was created")]
    public DateTime CreatedOn { get; set; }
    
    [Comment("Unique identifier of the user who last updated this note")]
    public Guid? UpdatedBy { get; set; }
    
    [Comment("Date and time when the record was last updated")]
    public DateTime? UpdatedOn { get; set; }
    [Comment("Unique identifier of the litigation request")]
    public LitigationRequestCategory LitigationRequestCategoryId { get; set; }


    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual ICollection<LitigationRequestNoteFile> LitigationRequestNoteFiles { get; set; } = new List<LitigationRequestNoteFile>();

    public virtual Citizen Citizen { get; set; } = null!;
    public virtual SysLitigationRequestCategory? LitigationRequestCategory { get; set; }


}

