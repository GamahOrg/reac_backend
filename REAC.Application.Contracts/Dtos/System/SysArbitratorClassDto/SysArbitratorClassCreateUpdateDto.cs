namespace REAC.Application.Contracts.Dtos.System.AreaDto;

public partial class SysArbitratorClassCreateUpdateDto : SysAreaDto
{
    public virtual ICollection<SysArbitratorClassTranslationCreateUpdateDto> Translations { get; set; } = new List<SysArbitratorClassTranslationCreateUpdateDto>();

}
