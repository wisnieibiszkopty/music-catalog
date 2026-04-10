using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Notification.Service.Core.Hubs;
using Notification.Service.Core.Models;

namespace Notification.Service.Core.Consumers;

public class AllAlbumsScrapedConsumer(
    IHubContext<NotificationHub> hubContext,
    ILogger<AllAlbumsScrapedConsumer> logger
    ) : IConsumer<AllAlbumsScraped>
{
    public async Task Consume(ConsumeContext<AllAlbumsScraped> context)
    {
        var message = context.Message;
        logger.LogInformation(
            "Consuming AllAlbumsScraped event. ArtistId: {ArtistId}. CorrelationId: {CorrelationId}", 
            message.ArtistId, 
            context.CorrelationId
        );
        
        await hubContext.Clients.All.SendAsync(MessageTypes.AlbumsSaved, message.ArtistId);
    }
}