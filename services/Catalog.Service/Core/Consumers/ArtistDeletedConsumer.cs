using Catalog.Service.Core.Services;
using Contracts;
using MassTransit;

namespace Catalog.Service.Core.Consumers;

public class ArtistDeletedConsumer(ILogger<ArtistDeletedConsumer> logger, ICatalogService catalogService) : IConsumer<ArtistDeleted>
{
    public async Task Consume(ConsumeContext<ArtistDeleted> context)
    {
        var artistId = context.Message.ArtistId;
        
        logger.LogInformation("Consuming SaveAlbumData event. ArtistId: {ArtistId}", artistId);
        
        await catalogService.DeleteAlbumsByArtistId(artistId);
    }
}