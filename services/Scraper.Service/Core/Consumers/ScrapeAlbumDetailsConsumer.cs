using Contracts;
using MassTransit;
using Scraper.Service.Core.MusicServiceClient;

namespace Scraper.Service.Core.Consumers;

public class ScrapeAlbumDetailsConsumer : IConsumer<ScrapeAlbumDetails>
{
    private readonly ILogger<ScrapeAlbumDetailsConsumer> _logger;
    private readonly IMusicServiceClient _client;
    
    public ScrapeAlbumDetailsConsumer(IMusicServiceClient client, ILogger<ScrapeAlbumDetailsConsumer> logger)
    {
        _logger = logger;
        _client = client;
    }
    
    public async Task Consume(ConsumeContext<ScrapeAlbumDetails> context)
    {
        var albumId = context.Message.AlbumId;
        var artistId = context.Message.ArtistId;
        var album = await _client.GetAlbumInfo(albumId, artistId);

        if (album == null)
        {
            _logger.LogError(
                "Failed consuming ScrapeAlbumDetails event. AlbumId: {AlbumId}, ArtistId: {ArtistId}",
                albumId,
                artistId
            );
            
            await context.Publish(new ScrapingFailed(context.Message.CorrelationId,
                $"Cannot scrap data for album with id: {albumId}"));
        }
        else
        {
            _logger.LogInformation(
                "Consumed ScrapeAlbumDetails event. AlbumId: {AlbumId}, ArtistId: {ArtistId}",
                albumId,
                artistId
            );
            
            await context.Publish(new SaveAlbumData(context.Message.CorrelationId, album));   
        }
    }
}