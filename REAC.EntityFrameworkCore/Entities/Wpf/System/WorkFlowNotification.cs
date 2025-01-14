using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowNotification : AuditedBaseEntity<Guid>
{
    public Guid WorkFlowNotificationPlanId { get; set; }

    public Guid WorkFlowNotificationDestinationTypeId { get; set; }

    public Guid WorkFlowNotificationTypeId { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid? DepartmentId { get; set; }

    public string? NotificationMessage { get; set; }

    public virtual ICollection<WorkFlowNotificationTranslation> Translations { get; set; } = new List<WorkFlowNotificationTranslation>();

    public virtual SysDepartment? Department { get; set; }

    public virtual SysEmployee? Employee { get; set; }

    public virtual WorkFlowNotificationDestinationType WorkFlowNotificationDestinationType { get; set; } = null!;

    public virtual WorkFlowNotificationPlan WorkFlowNotificationPlan { get; set; } = null!;

    public virtual WorkFlowNotificationType WorkFlowNotificationType { get; set; } = null!;
}
