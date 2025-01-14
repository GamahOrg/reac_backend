using REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;

public partial class SysLitigationRequestCategoryCreateUpdateDto : SysLitigationRequestCategoryDto
{
    /// <summary>
    /// صلح – تحكيم-صلح وتحكيم
    /// </summary>
    public virtual ICollection<SysLitigationRequestCategoryTranslationCreateUpdateDto> Translations { get; set; } = new List<SysLitigationRequestCategoryTranslationCreateUpdateDto>();

}
