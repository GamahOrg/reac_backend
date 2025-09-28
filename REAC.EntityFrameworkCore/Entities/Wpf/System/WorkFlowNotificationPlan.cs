using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowNotificationPlan : AuditedBaseEntity<Guid>
{
    [Comment("Description for Notification Plan")]
    public string? NotificationPlan { get; set; }
    public virtual ICollection<WorkFlowNotificationPlanTranslation> Translations { get; set; } = new List<WorkFlowNotificationPlanTranslation>();


    public virtual ICollection<WorkFlowStepEvent> WorkFlowStepEventCompletionNotificationPlans { get; set; } = new List<WorkFlowStepEvent>();

    public virtual ICollection<WorkFlowStepEvent> WorkFlowStepEventCreationNotificationPlans { get; set; } = new List<WorkFlowStepEvent>();

    public virtual ICollection<WorkFlowNotification> WorkFlowNotifications { get; set; } = new List<WorkFlowNotification>();
}
