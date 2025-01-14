using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.Notification
{
    public class SysRequestType : FullAuditedBaseEntity<RequestType>
    {
        public required string RequestTypeName { get; set; }
        public virtual ICollection<SysNotification> SysNotifications { get; } = new List<SysNotification>();
    }
}
