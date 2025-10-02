using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowStepEventSecurityLevelActor : BaseEntity<Guid>
{
    [Comment("Unique identifier of the work flow step event")]
    public Guid WorkFlowStepEventId { get; set; }

    [Comment("Unique identifier of the security level actor")]
    public SecurityLevelActorType SecurityLevelActorId { get; set; }

    [Comment("Unique identifier of the department")]
    public Guid? DepartmentId { get; set; }

    [Comment("Unique identifier of the employee")]
    public Guid? EmployeeId { get; set; }

    public virtual WorkFlowStepEventSecurityLevelActorType WorkFlowStepEventSecurityLevelActorType { get; set; } = null!;

    public virtual SysDepartment? Department { get; set; }

    public virtual SysEmployee? Employee { get; set; }

    public virtual WorkFlowStepEvent WorkFlowStepEvent { get; set; } = null!;

}

