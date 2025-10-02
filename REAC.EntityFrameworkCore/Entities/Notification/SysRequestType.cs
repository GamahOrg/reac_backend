using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Notification
{
    public class SysRequestType : FullAuditedBaseEntity<RequestType>
    {
        [Comment("Type of the name")]
        public required string RequestTypeName { get; set; }
        public virtual ICollection<SysNotification> SysNotifications { get; } = new List<SysNotification>();
    }
}
