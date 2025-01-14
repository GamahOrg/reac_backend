using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestRequest : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public string RequestContent { get; set; } = null!;

    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual ICollection<LitigationRequestRequestFile> LitigationRequestRequestFiles { get; set; } = new List<LitigationRequestRequestFile>();
}
