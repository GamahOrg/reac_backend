using REAC.Application.Contracts.Dtos.System.LitigationRequestOwnerTypeDtos;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestOwnerTypeDtos;

public partial class SysLitigationRequestOwnerTypeCreateUpdateDto : SysLitigationRequestOwnerTypeDto
{
    /// <summary>
    /// مقدم عن نفسه – مفوض
    /// </summary>
    public virtual ICollection<SysLitigationRequestOwnerTypeTranslationCreateUpdateDto> Translations { get; set; } = new List<SysLitigationRequestOwnerTypeTranslationCreateUpdateDto>();
}
