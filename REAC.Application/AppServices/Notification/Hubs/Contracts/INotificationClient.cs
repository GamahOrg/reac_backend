using REAC.Application.Contracts.Dtos.Notification;

namespace Wafi.Application.Hubs.Contracts;

public interface INotificationClient
{
    Task NotifyCitizen(SysNotificationDto sysNotification);
    Task NotifyEmployee(SysNotificationDto sysNotification);
}
