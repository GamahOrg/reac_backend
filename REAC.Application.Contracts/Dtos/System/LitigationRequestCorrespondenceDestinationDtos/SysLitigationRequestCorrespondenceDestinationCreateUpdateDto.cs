using REAC.Application.Contracts.Dtos.System.LitigationRequestCorrespondenceDestinationDtos;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestCorrespondenceDestinationDtos;

public partial class SysLitigationRequestCorrespondenceDestinationCreateUpdateDto : SysLitigationRequestCorrespondenceDestinationDto
{
    public virtual ICollection<SysLitigationRequestCorrespondenceDestinationTranslationCreateUpdateDto> Translations { get; set; } = new List<SysLitigationRequestCorrespondenceDestinationTranslationCreateUpdateDto>();
}
