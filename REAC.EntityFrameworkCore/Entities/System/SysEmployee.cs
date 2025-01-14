using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysEmployee : FullAuditedBaseEntity<Guid>
{
    public Guid DepartmentId { get; set; }

    public string EmployeeName { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public bool AllowAssign { get; set; }

    public bool WorkAsEmployee { get; set; }

    public bool AllowReAssign { get; set; }

    public string Mobile { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Guid? UserId { get; set; }

    public bool? IsLocked { get; set; }

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
