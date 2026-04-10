using Catalog.Service.Core.Models;
using Catalog.Service.Core.Services;
using Contracts;
using MassTransit;

namespace Catalog.Service.Core.Consumers;

public class SaveAlbumDataConsumer(ILogger<SaveAlbumDataConsumer> logger, ICatalogService catalogService) : IConsumer<SaveAlbumData>
{
    public async Task Consume(ConsumeContext<SaveAlbumData> context)
    {
        var message = context.Message;
        
        logger.LogInformation("Consuming SaveAlbumData event. CorrelationId: {CorrelationId}, AlbumName: {AlbumName}", 
            message.CorrelationId, message.AlbumDetails?.Name);
        
        var album = new Album(message.AlbumDetails);
        var createdAlbum = await catalogService.Create(album);
        
        await context.Publish(new AlbumSaved(context.Message.CorrelationId, createdAlbum.Id ?? string.Empty));
    }
}