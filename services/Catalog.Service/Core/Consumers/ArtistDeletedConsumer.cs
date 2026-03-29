using Catalog.Service.Core.Services;
using Contracts;
using MassTransit;

namespace Catalog.Service.Core.Consumers;

public class ArtistDeletedConsumer(ICatalogService catalogService) : IConsumer<ArtistDeleted>
{
    public async Task Consume(ConsumeContext<ArtistDeleted> context)
    {
        var artistId = context.Message.ArtistId;
        await catalogService.DeleteAlbumsByArtistId(artistId);
    }
}