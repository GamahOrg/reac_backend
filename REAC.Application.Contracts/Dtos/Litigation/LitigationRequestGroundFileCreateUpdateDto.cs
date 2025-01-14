using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestGroundFileCreateUpdateDto : BaseEntity<Guid>
{
    public Guid LitigationRequestGroundId { get; set; }

    public Guid FileManagerId { get; set; }
}
