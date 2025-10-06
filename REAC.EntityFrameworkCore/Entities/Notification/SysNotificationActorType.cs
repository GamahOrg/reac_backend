using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Notification;

public class SysNotificationActorType : BaseEntity<NotificationActorType>
{
    [Comment("Description for Actor Type")]
    public required string ActorType { get; set; }
    public virtual ICollection<SysNotification> SysNotifications { get; set; } = new List<SysNotification>();
    public virtual ICollection<SysNotificationPlanItemDestination> SysNotificationPlanItemDestinations { get; set; } = new List<SysNotificationPlanItemDestination>();


}

