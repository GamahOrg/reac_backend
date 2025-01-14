namespace REAC.Application.Contracts.Dtos.System.EmployeeDtos;

public partial class SysEmployeeCreateUpdateDto : SysEmployeeDto
{
    public virtual ICollection<SysEmployeeTranslationCreateUpdateDto> Translations { get; set; } = new List<SysEmployeeTranslationCreateUpdateDto>();

}
