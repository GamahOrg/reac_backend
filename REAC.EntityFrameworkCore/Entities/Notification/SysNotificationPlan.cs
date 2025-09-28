using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Notification
{
    public class SysNotificationPlan : BaseEntity<Guid>
    {
        [Comment("Name of the notification plan")]
        public string NotificationPlanName { get; set; } = null!;
        public virtual ICollection<SysNotificationPlanItem> SysNotificationPlanItems { get; set; } = new List<SysNotificationPlanItem>();
    }



}

