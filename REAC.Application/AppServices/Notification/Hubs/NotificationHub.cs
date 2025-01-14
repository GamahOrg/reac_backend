using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Wafi.Application.Hubs.Contracts;

namespace Wafi.Application.Hubs;

[AllowAnonymous]
public class NotificationHub : Hub<INotificationClient>
{

    public NotificationHub()
    {

    }
    public override Task OnConnectedAsync()
    {

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}
