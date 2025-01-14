namespace REAC.Application.Contracts.Dtos.System.GenderDtos;

public partial class SysGenderCreateUpdateDto : SysGenderDto
{
    public virtual ICollection<SysGenderTranslationCreateUpdateDto> Translations { get; set; } = new List<SysGenderTranslationCreateUpdateDto>();
}
