using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowNotification : AuditedBaseEntity<Guid>
{
    [Comment("Unique identifier of the work flow notification plan")]
    public Guid WorkFlowNotificationPlanId { get; set; }

    [Comment("Unique identifier of the work flow notification destination type")]
    public Guid WorkFlowNotificationDestinationTypeId { get; set; }

    [Comment("Unique identifier of the work flow notification type")]
    public Guid WorkFlowNotificationTypeId { get; set; }

    [Comment("Unique identifier of the employee")]
    public Guid? EmployeeId { get; set; }

    [Comment("Unique identifier of the department")]
    public Guid? DepartmentId { get; set; }

    [Comment("Content of the notification message")]
    public string? NotificationMessage { get; set; }

    public virtual ICollection<WorkFlowNotificationTranslation> Translations { get; set; } = new List<WorkFlowNotificationTranslation>();

    public virtual SysDepartment? Department { get; set; }

    public virtual SysEmployee? Employee { get; set; }

    public virtual WorkFlowNotificationDestinationType WorkFlowNotificationDestinationType { get; set; } = null!;

    public virtual WorkFlowNotificationPlan WorkFlowNotificationPlan { get; set; } = null!;

    public virtual WorkFlowNotificationType WorkFlowNotificationType { get; set; } = null!;
}

