using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Notification
{
    public class SysNotificationPlan : BaseEntity<Guid>
    {
        public string NotificationPlanName { get; set; } = null!;
        public virtual ICollection<SysNotificationPlanItem> SysNotificationPlanItems { get; set; } = new List<SysNotificationPlanItem>();
    }



}
