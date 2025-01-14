namespace REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;

public partial class SysExemptionReductionCategoryCreateUpdateDto : SysExemptionReductionCategoryDto
{
    public virtual ICollection<SysExemptionReductionCategoryTranslationCreateUpdateDto> Translations { get; set; } = new List<SysExemptionReductionCategoryTranslationCreateUpdateDto>();

}
