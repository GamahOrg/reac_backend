using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Notification;

public class NotificationDto : BaseEntity<Guid>
{
    public int RequestTypeId { get; set; }

    public Guid RequestId { get; set; }

    public NotificationActorType NotificationActorTypeId { get; set; }

    public Guid ActorId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public bool IsViewed { get; set; }

    public DateTime? ViewedOn { get; set; }
    public string SysNotificationActorType { get; set; } = null!;
    public string SysRequestType { get; set; }

}
