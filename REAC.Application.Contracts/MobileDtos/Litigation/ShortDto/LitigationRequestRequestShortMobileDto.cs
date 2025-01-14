using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.Contracts.MobileDtos.Litigation.ShortDto;

public partial class LitigationRequestRequestShortMobileDto : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public string RequestContent { get; set; } = null!;


    public virtual ICollection<LitigationRequestRequestFile> LitigationRequestRequestFiles { get; set; } = new List<LitigationRequestRequestFile>();
}
