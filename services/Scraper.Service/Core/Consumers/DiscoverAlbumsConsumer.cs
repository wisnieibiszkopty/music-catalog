using Contracts;
using MassTransit;
using Scraper.Service.Core.MusicServiceClient;

namespace Scraper.Service.Core.Consumers;

public class DiscoverAlbumsConsumer : IConsumer<DiscoverAlbums>
{
    private readonly IMusicServiceClient _client;
    
    public DiscoverAlbumsConsumer(IMusicServiceClient client)
    {
        _client = client;
    }
    
    public async Task Consume(ConsumeContext<DiscoverAlbums> context)
    {
        var artistId = context.Message.ArtistId;
        try
        {
            var albums = await _client.GetAlbumsByArtistId(artistId);
            await context.Publish(new AlbumsDiscovered(context.Message.CorrelationId, albums));
        }
        catch (MusicServiceRateLimitException exception)
        {
            await context.Publish(new ScrapingFailed(context.Message.CorrelationId, exception.Message));
        }
    }
}