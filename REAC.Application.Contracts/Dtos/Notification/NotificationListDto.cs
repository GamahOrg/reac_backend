namespace REAC.Application.Contracts.Dtos.Notification;

public class NotificationListDto
{
    public int NotViewedCount { get; set; }
    public List<NotificationDto> NotificationList { get; set; }
}
