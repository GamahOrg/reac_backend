using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestRequest : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request")]
    public Guid LitigationRequestId { get; set; }

    [Comment("Content")]
    public string RequestContent { get; set; } = null!;

    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual ICollection<LitigationRequestRequestFile> LitigationRequestRequestFiles { get; set; } = new List<LitigationRequestRequestFile>();
}

