using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using REAC.Application.AppServices.Integration.Hubs.Data;
using REAC.Application.Contracts.IAppServices.Integration.Hubs.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace REAC.Application.AppServices.Integration.Hubs
{
    [AllowAnonymous]
    [EnableCors("CorsPolicy")]
    public class NafathReconciliationReportHub : Hub<INafathReconciliationReportStatus>
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public async Task Register(string transactionId)
        {
            string? connectionId = SignalRConnectionToGroupsMap.GetGroupConnection(transactionId);
            if (connectionId != null)
            {
                await Groups.RemoveFromGroupAsync(connectionId, transactionId);
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, transactionId);
            SignalRConnectionToGroupsMap.AddGroup(transactionId, Context.ConnectionId);
        }
        public async Task UnRegister(string transactionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, transactionId);
            SignalRConnectionToGroupsMap.RemoveGroup(transactionId);
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
