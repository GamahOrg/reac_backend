using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequestNote : BaseEntity<Guid>
{
    public Guid CitizenRequestId { get; set; }
    public string Notes { get; set; } = null!;
    public DateTime NoteDateTime { get; set; }

    public virtual CitizenRequest CitizenRequest { get; set; } = null!;

}
