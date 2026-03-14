using Contracts;
using MassTransit;

namespace Catalog.Service.Core.Consumers;

public class SaveAlbumDataConsumer : IConsumer<SaveAlbumData>
{
    public async Task Consume(ConsumeContext<SaveAlbumData> context)
    {
        var albumDetails = context.Message.AlbumDetails;
        
        Console.WriteLine(albumDetails.Name);
        Console.WriteLine(albumDetails.ArtistId);
        
        Console.WriteLine(String.Join(", ", albumDetails.Tracks.Select(t => t.Name)));
        
        // TODO save album details
        await context.Publish(new AlbumSaved(context.Message.CorrelationId, albumDetails.Id));
    }
}