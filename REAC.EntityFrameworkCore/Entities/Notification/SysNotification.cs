using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.Notification;


public class SysNotification : BaseEntity<Guid>
{
    public RequestType RequestTypeId { get; set; }

    public Guid RequestId { get; set; }

    public NotificationActorType NotificationActorTypeId { get; set; }

    public Guid ActorId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public bool IsViewed { get; set; }

    public DateTime? ViewedOn { get; set; }
    public virtual SysNotificationActorType SysNotificationActorType { get; set; } = null!;
    public virtual SysRequestType SysRequestType { get; set; } = null!;

}
