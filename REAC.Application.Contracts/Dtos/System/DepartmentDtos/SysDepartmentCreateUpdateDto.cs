namespace REAC.Application.Contracts.Dtos.System.DepartmentDtos;

public partial class SysDepartmentCreateUpdateDto : SysDepartmentDto
{
    public virtual ICollection<SysDepartmentTranslationCreateUpdateDto> Translations { get; set; } = new List<SysDepartmentTranslationCreateUpdateDto>();

}
