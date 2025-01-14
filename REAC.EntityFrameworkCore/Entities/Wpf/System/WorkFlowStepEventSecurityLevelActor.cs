using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepEventSecurityLevelActor : BaseEntity<Guid>
{
    public Guid WorkFlowStepEventId { get; set; }

    public SecurityLevelActorType SecurityLevelActorId { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? EmployeeId { get; set; }

    public virtual WorkFlowStepEventSecurityLevelActorType WorkFlowStepEventSecurityLevelActorType { get; set; } = null!;

    public virtual SysDepartment? Department { get; set; }

    public virtual SysEmployee? Employee { get; set; }

    public virtual WorkFlowStepEvent WorkFlowStepEvent { get; set; } = null!;

}
