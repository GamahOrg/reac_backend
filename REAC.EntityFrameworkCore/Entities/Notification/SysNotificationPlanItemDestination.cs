
using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Notification
{
    public class SysNotificationPlanItemDestination : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the sys notification plan item")]
        public Guid SysNotificationPlanItemId { get; set; }

        [Comment("Description for Notification Destination Type")]
        public NotificationDestinationType NotificationDestinationType { get; set; }

        [Comment("Unique identifier of the department")]
        public Guid? DepartmentId { get; set; }

        [Comment("Unique identifier of the notification actor type")]
        public NotificationActorType NotificationActorTypeId { get; set; }

        [Comment("Notification Type")]
        public NotificationType NotificationType { get; set; }

        public virtual SysDepartment? SysDepartment { get; set; }

        public virtual SysNotificationActorType NotificationActorType { get; set; } = null!;

        public virtual SysNotificationPlanItem SysNotificationPlanItem { get; set; } = null!;
    }
}

