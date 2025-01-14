namespace REAC.Application.Contracts.Dtos.System.AreaDto;

public partial class SysAreaCreateUpdateDto : SysAreaDto
{
    public virtual ICollection<SysAreaTranslationCreateUpdateDto> Translations { get; set; } = new List<SysAreaTranslationCreateUpdateDto>();

}
