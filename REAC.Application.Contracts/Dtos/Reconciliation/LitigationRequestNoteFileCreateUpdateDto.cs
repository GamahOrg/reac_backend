using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestNoteFileCreateUpdateDto : BaseEntity<Guid>
{
    public Guid NoteId { get; set; }

    public Guid FileManagerId { get; set; }

}
