using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Notification
{
    public class SysNotificationPlanItem : BaseEntity<Guid>
    {
        public Guid SysNotificationPlanId { get; set; }

        public string? EmailSubject { get; set; }
        public string? MailTemplate { get; set; }
        public string? SmsTemplate { get; set; }
        public string? ApplicationNotificationTemplate { get; set; }

        public virtual SysNotificationPlan SysNotificationPlan { get; set; } = null!;
        public virtual ICollection<SysNotificationPlanItemDestination> SysNotificationPlanItemDestinations { get; set; } = new List<SysNotificationPlanItemDestination>();

    }
}
