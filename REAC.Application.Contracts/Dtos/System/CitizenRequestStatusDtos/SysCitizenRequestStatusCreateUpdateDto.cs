namespace REAC.Application.Contracts.Dtos.System.CitizenRequestStatusDtos;

public partial class SysCitizenRequestStatusCreateUpdateDto : SysCitizenRequestStatusDto
{
    public virtual ICollection<SysCitizenRequestStatusTranslationCreateUpdateDto> Translations { get; set; } = new List<SysCitizenRequestStatusTranslationCreateUpdateDto>();

}
