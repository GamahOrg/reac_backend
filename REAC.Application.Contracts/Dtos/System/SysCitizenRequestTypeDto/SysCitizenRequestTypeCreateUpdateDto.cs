namespace REAC.Application.Contracts.Dtos.System.AreaDto;

public partial class SysCitizenRequestTypeCreateUpdateDto : SysAreaDto
{
    public virtual ICollection<SysCitizenRequestTypeTranslationCreateUpdateDto> Translations { get; set; } = new List<SysCitizenRequestTypeTranslationCreateUpdateDto>();

}
