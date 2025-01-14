using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestNote : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public string? Title { get; set; }

    public string Notes { get; set; } = null!;

    public Guid? CreatedBy { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public Guid? UpdatedBy { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
    public LitigationRequestCategory LitigationRequestCategoryId { get; set; }


    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual ICollection<LitigationRequestNoteFile> LitigationRequestNoteFiles { get; set; } = new List<LitigationRequestNoteFile>();

    public virtual Citizen Citizen { get; set; } = null!;
    public virtual SysLitigationRequestCategory? LitigationRequestCategory { get; set; }


}
