using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestRequestCreateUpdateDto : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }
    public string RequestContent { get; set; } = null!;
}
