using Contracts;
using MassTransit;

namespace Notification.Service.Core.Consumers;

public class ArtistSavedConsumer : IConsumer<ArtistSaved>
{
    public Task Consume(ConsumeContext<ArtistSaved> context)
    {
        Console.WriteLine(context.Message.ArtistId);
        return Task.CompletedTask;
    }
}