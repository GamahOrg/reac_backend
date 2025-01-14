using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestCorrespondenceDestinationDtos;

public partial class SysLitigationRequestCorrespondenceDestinationDto : BaseEntity<LitigationRequestCorrespondenceDestination>
{
    public string Destination { get; set; } = null!;


}
