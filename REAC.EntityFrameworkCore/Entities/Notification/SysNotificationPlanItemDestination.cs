
using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Notification
{
    public class SysNotificationPlanItemDestination : BaseEntity<Guid>
    {
        public Guid SysNotificationPlanItemId { get; set; }

        public NotificationDestinationType NotificationDestinationType { get; set; }

        public Guid? DepartmentId { get; set; }

        public NotificationActorType NotificationActorTypeId { get; set; }

        public NotificationType NotificationType { get; set; }

        public virtual SysDepartment? SysDepartment { get; set; }

        public virtual SysNotificationActorType NotificationActorType { get; set; } = null!;

        public virtual SysNotificationPlanItem SysNotificationPlanItem { get; set; } = null!;
    }
}
