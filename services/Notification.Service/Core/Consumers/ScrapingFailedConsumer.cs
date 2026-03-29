using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Service.Core.Hubs;
using Notification.Service.Core.Models;

namespace Notification.Service.Core.Consumers;

public class ScrapingFailedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<ScrapingFailed>
{
    public async Task Consume(ConsumeContext<ScrapingFailed> context)
    {
        var message = context.Message.ErrorMessage;
        await hubContext
            .Clients
            // TODO replace with admin group
            .All
            .SendAsync(MessageTypes.ScrapingFailed, message);
    }
}