using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Notification;

public class SysNotificationDto : BaseEntity<Guid>
{
    public Guid ActorId { get; set; }
    public Guid RequestId { get; set; }
    public RequestType RequestTypeId { get; set; }
    public NotificationActorType ActorTypeId { get; set; }
    public string Message { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
    public bool IsViewed { get; set; }
    public DateTime? ViewedOn { get; set; }
}
