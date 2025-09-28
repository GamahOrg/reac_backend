using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using REAC.EntityFrameworkCore.Entities.Wpf.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysEmployee : FullAuditedBaseEntity<Guid>
{
    [Comment("Unique identifier of the department")]
    public Guid DepartmentId { get; set; }

    [Comment("Name of the employee")]
    public string EmployeeName { get; set; } = null!;

    [Comment("Indicates whether the employee is an administrator")]
    public bool IsAdmin { get; set; }

    [Comment("Indicates whether the employee can assign tasks")]
    public bool AllowAssign { get; set; }

    [Comment("Indicates whether the person works as an employee")]
    public bool WorkAsEmployee { get; set; }

    [Comment("Indicates whether the employee can reassign tasks")]
    public bool AllowReAssign { get; set; }

    [Comment("Mobile phone number")]
    public string Mobile { get; set; } = null!;

    [Comment("Email address")]
    public string Email { get; set; } = null!;

    [Comment("Unique identifier of the user")]
    public Guid? UserId { get; set; }

    [Comment("Indicates whether the employee account is locked")]
    public bool? IsLocked { get; set; }

    [Comment("Indicates whether the record is active")]
    public bool? IsActive { get; set; }

    public virtual SysDepartment Department { get; set; } = null!;

    public virtual ICollection<SysEmployeeTranslation> Translations { get; set; } = new List<SysEmployeeTranslation>();

    public virtual ICollection<SysVacationRequest> EmployeeVacationRequests { get; set; } = new List<SysVacationRequest>();

    public virtual ICollection<SysVacationRequest> EmployeeApprovedVacationRequests { get; set; } = new List<SysVacationRequest>();

    public virtual ICollection<LitigationRequestExtendPeriodRequest> LitigationRequestExtendPeriodRequests { get; set; } = new List<LitigationRequestExtendPeriodRequest>();

    public virtual ICollection<WorkFlowNotification> WorkFlowNotifications { get; set; } = new List<WorkFlowNotification>();

    public virtual ICollection<WorkFlowProcessingLog> WorkFlowProcessingLogs { get; set; } = new List<WorkFlowProcessingLog>();

    public virtual ICollection<WorkFlowStep> WorkFlowSteps { get; set; } = new List<WorkFlowStep>();

    public virtual ICollection<WorkFlowStepEventSecurityLevelActor> WorkFlowStepEventSecurityLevelActors { get; set; } = new List<WorkFlowStepEventSecurityLevelActor>();
}

