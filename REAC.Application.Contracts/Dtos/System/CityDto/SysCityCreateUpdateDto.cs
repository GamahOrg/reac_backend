namespace REAC.Application.Contracts.Dtos.System.AreaDto;

public partial class SysCityCreateUpdateDto : SysAreaDto
{
    public virtual ICollection<SysCityTranslationCreateUpdateDto> Translations { get; set; } = new List<SysCityTranslationCreateUpdateDto>();

}
