using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Wpf.System;

public partial class WorkFlowNotificationType : BaseEntity<Guid>
{

    public string NotificationType { get; set; } = null!;

    public virtual ICollection<WorkFlowNotificationTypeTranslation> Translations { get; set; } = new List<WorkFlowNotificationTypeTranslation>();

    public virtual ICollection<WorkFlowNotification> WorkFlowNotifications { get; set; } = new List<WorkFlowNotification>();
}
