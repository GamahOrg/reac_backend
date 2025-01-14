using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestRequestFileCreateUpdateDto : BaseEntity<Guid>
{
    public Guid LitigationRequestRequestId { get; set; }

    public Guid FileManagerId { get; set; }
}
