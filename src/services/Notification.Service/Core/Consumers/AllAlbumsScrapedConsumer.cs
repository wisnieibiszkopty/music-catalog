using Contracts;
using MassTransit;

namespace Notification.Service.Core.Consumers;

public class AllAlbumsScrapedConsumer : IConsumer<AllAlbumsScraped>
{
    public Task Consume(ConsumeContext<AllAlbumsScraped> context)
    {
        Console.WriteLine(context.Message.ArtistId);
        return Task.CompletedTask;
    }
}