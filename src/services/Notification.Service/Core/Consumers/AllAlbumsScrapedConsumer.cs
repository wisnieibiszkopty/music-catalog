using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Service.Core.Hubs;
using Notification.Service.Core.Models;

namespace Notification.Service.Core.Consumers;

public class AllAlbumsScrapedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<AllAlbumsScraped>
{
    public async Task Consume(ConsumeContext<AllAlbumsScraped> context)
    {
        var artistId = context.Message.ArtistId;
        await hubContext.Clients.All.SendAsync(MessageTypes.AlbumsSaved, artistId);
    }
}