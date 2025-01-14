using REAC.Application.Contracts.Dtos.System.LitigationRequestStatusDtos;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestStatusDtos;

public partial class SysLitigationRequestStatusCreateUpdateDto : SysLitigationRequestStatusDto
{
    public virtual ICollection<SysLitigationRequestStatusTranslationCreateUpdateDto> Translations { get; set; } = new List<SysLitigationRequestStatusTranslationCreateUpdateDto>();

}
