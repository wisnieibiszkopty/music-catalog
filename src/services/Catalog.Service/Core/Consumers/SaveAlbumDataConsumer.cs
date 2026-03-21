using Catalog.Service.Core.Models;
using Catalog.Service.Core.Services;
using Contracts;
using MassTransit;

namespace Catalog.Service.Core.Consumers;

public class SaveAlbumDataConsumer(ICatalogService catalogService) : IConsumer<SaveAlbumData>
{
    public async Task Consume(ConsumeContext<SaveAlbumData> context)
    {
        var albumDetails = context.Message.AlbumDetails;
        
        Console.WriteLine(albumDetails.Name);
        Console.WriteLine(albumDetails.ArtistId);
        
        Console.WriteLine(String.Join(", ", albumDetails.Tracks.Select(t => t.Name)));

        var album = new Album(albumDetails);
        var createdAlbum = await catalogService.Create(album);
        
        await context.Publish(new AlbumSaved(context.Message.CorrelationId, createdAlbum.Id ?? string.Empty));
    }
}