using Microsoft.AspNetCore.SignalR;

namespace Notification.Service.Core.Hubs;

public class NotificationHub(ILogger<NotificationHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User.IsInRole("Admin"))
        {
            logger.LogInformation("Connected to admin user");
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
        }

        await base.OnConnectedAsync();
    }
}