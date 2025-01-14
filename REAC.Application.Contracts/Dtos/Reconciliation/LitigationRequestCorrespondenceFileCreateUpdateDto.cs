using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestCorrespondenceFileCreateUpdateDto : BaseEntity<Guid>
{
    public Guid LitigationRequestCorrespondenceId { get; set; }

    public Guid FileManagerId { get; set; }

}
