using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System.EmployeeDtos;

public partial class SysEmployeeDto : BaseEntity<Guid>
{
    public Guid DepartmentId { get; set; }

    public string EmployeeName { get; set; } = null!;


    public bool IsAdmin { get; set; }

    public bool WorkAsEmployee { get; set; }

    public bool AllowAssign { get; set; }

    public bool AllowReAssign { get; set; }

    public string Mobile { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Guid? UserId { get; set; }

    public bool? IsLocked { get; set; }

    public bool? IsActive { get; set; }

}
