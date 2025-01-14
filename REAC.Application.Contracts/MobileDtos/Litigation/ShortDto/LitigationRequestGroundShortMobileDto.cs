using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.Contracts.MobileDtos.Litigation.ShortDto;

public class LitigationRequestGroundShortMobileDto : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public string? Title { get; set; }


    public virtual ICollection<LitigationRequestGroundFile> LitigationRequestGroundFiles { get; set; } = new List<LitigationRequestGroundFile>();
}
