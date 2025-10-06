using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Notification;


public class SysNotification : BaseEntity<Guid>
{
    [Comment("Unique identifier of the request type")]
    public RequestType RequestTypeId { get; set; }

    [Comment("Unique identifier of the request")]
    public Guid RequestId { get; set; }

    [Comment("Unique identifier of the notification actor type")]
    public NotificationActorType NotificationActorTypeId { get; set; }

    [Comment("Unique identifier of the actor")]
    public Guid ActorId { get; set; }

    [Comment("Description for Message")]
    public string Message { get; set; } = null!;

    [Comment("Date and time when the record was created")]
    public DateTime CreatedOn { get; set; }

    [Comment("Description for Is Viewed")]
    public bool IsViewed { get; set; }

    [Comment("Date and time when the notification was viewed")]
    public DateTime? ViewedOn { get; set; }
    public virtual SysNotificationActorType SysNotificationActorType { get; set; } = null!;
    public virtual SysRequestType SysRequestType { get; set; } = null!;

}

