using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Service.Core.Hubs;
using Notification.Service.Core.Models;

namespace Notification.Service.Core.Consumers;

public class ArtistSavedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<ArtistSaved>
{
    public async Task Consume(ConsumeContext<ArtistSaved> context)
    {
        var artist = context.Message.Artist;
        await hubContext.Clients.All.SendAsync(MessageTypes.ArtistSaved, artist);
    }
}