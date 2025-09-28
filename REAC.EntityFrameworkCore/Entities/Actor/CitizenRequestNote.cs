using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestNote : BaseEntity<Guid>
{
    [Comment("Unique identifier of the citizen request")]
    public Guid CitizenRequestId { get; set; }
    [Comment("Notes or comments about the citizen request")]
    public string Notes { get; set; } = null!;
    [Comment("Date and time of the note")]
    public DateTime NoteDateTime { get; set; }

    public virtual CitizenRequest CitizenRequest { get; set; } = null!;

}


