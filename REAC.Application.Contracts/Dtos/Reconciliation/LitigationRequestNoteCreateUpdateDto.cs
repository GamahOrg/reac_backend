using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestNoteCreateUpdateDto : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public string? Title { get; set; }

    public string Notes { get; set; } = null!;
    public Guid? NotesFileId { get; set; } = null!;

    public Guid? CreatedBy { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public Guid? UpdatedBy { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
    public LitigationRequestCategory LitigationRequestCategoryId { get; set; }
}
