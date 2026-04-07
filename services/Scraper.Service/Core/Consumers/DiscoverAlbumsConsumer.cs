using Contracts;
using MassTransit;
using Scraper.Service.Core.MusicServiceClient;

namespace Scraper.Service.Core.Consumers;

public class DiscoverAlbumsConsumer : IConsumer<DiscoverAlbums>
{
    private readonly IMusicServiceClient _client;
    private readonly ILogger<DiscoverAlbumsConsumer> _logger;
    
    public DiscoverAlbumsConsumer(IMusicServiceClient client, ILogger<DiscoverAlbumsConsumer> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<DiscoverAlbums> context)
    {
        var artistId = context.Message.ArtistId;
        try
        {
            var albums = await _client.GetAlbumsByArtistId(artistId);
            _logger.LogInformation("Consumed DiscoverAlbums event. ArtistId: {ArtistId}", artistId);
            await context.Publish(new AlbumsDiscovered(context.Message.CorrelationId, albums));
        }
        catch (MusicServiceRateLimitException exception)
        {
            _logger.LogError(
                "Failed to consume DiscoverAlbum event. ArtistId: {ArtistId}, message: {Message}",
                artistId,
                exception.Message
            );
            
            await context.Publish(new ScrapingFailed(context.Message.CorrelationId, exception.Message));
        }
    }
}