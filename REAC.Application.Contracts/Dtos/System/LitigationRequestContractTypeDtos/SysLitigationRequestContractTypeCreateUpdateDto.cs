namespace REAC.Application.Contracts.Dtos.System.LitigationRequestContractTypeDtos;

public partial class SysLitigationRequestContractTypeCreateUpdateDto : SysLitigationRequestContractTypeDto
{
    public virtual ICollection<SysLitigationRequestContractTypeTranslationCreateUpdateDto> Translations { get; set; } = new List<SysLitigationRequestContractTypeTranslationCreateUpdateDto>();
}
