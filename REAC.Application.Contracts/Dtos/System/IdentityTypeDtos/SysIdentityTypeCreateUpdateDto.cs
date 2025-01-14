namespace REAC.Application.Contracts.Dtos.System.IdentityTypeDtos;

public partial class SysIdentityTypeCreateUpdateDto : SysIdentityTypeDto
{
    public virtual ICollection<SysIdentityTypeTranslationCreateUpdateDto> Translations { get; set; } = new List<SysIdentityTypeTranslationCreateUpdateDto>();
}
