using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Notification;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysDepartment : FullAuditedBaseEntity<Guid>
{
    public string DepartmentName { get; set; } = null!;

    public DepartmentDistributionMode DistributionMode { get; set; }
    public DepartmentType DepartmentType { get; set; }
    public virtual ICollection<SysDepartmentTranslation> Translations { get; set; } = new List<SysDepartmentTranslation>();

    public virtual ICollection<SysEmployee> SysEmployees { get; set; } = new List<SysEmployee>();

    public virtual ICollection<WorkFlowStepEventSecurityLevelActor> WorkFlowStepEventSecurityLevelActors { get; set; } = new List<WorkFlowStepEventSecurityLevelActor>();
    public virtual ICollection<SysNotificationPlanItem> SysNotificationPlanItems { get; set; } = new List<SysNotificationPlanItem>();

    public virtual ICollection<WorkFlowNotification> WorkFlowNotifications { get; set; } = new List<WorkFlowNotification>();

    public virtual ICollection<WorkFlowStep> WorkFlowSteps { get; set; } = new List<WorkFlowStep>();

    public virtual ICollection<WorkFlow> WorkFlows { get; set; } = new List<WorkFlow>();
}

