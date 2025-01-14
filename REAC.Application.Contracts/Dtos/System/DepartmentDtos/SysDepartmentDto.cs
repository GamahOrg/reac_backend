using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System.DepartmentDtos;

public partial class SysDepartmentDto : BaseEntity<Guid>
{
    public string DepartmentName { get; set; } = null!;

    public bool AutoDistribution { get; set; }

}
