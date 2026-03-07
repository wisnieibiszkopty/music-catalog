using Contracts;
using MassTransit;

namespace Catalog.Service.Core.Consumers;

public class SaveAlbumDataConsumer : IConsumer<SaveAlbumData>
{
    public Task Consume(ConsumeContext<SaveAlbumData> context)
    {
        var albumDetails = context.Message.AlbumDetails;
        
        // TODO save album details
        context.Publish(new AlbumSaved(context.Message.CorrelationId, albumDetails.Id));

        return Task.CompletedTask;
    }
}