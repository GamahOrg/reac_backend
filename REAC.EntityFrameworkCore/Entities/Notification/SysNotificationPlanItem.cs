using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Notification
{
    public class SysNotificationPlanItem : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the sys notification plan")]
        public Guid SysNotificationPlanId { get; set; }

        [Comment("Email address")]
        public string? EmailSubject { get; set; }
        [Comment("Description for Mail Template")]
        public string? MailTemplate { get; set; }
        [Comment("Description for Sms Template")]
        public string? SmsTemplate { get; set; }
        [Comment("Description for Application Notification Template")]
        public string? ApplicationNotificationTemplate { get; set; }

        public virtual SysNotificationPlan SysNotificationPlan { get; set; } = null!;
        public virtual ICollection<SysNotificationPlanItemDestination> SysNotificationPlanItemDestinations { get; set; } = new List<SysNotificationPlanItemDestination>();

    }
}

