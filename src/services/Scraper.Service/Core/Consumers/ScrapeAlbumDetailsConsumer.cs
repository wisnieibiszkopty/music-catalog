using Contracts;
using MassTransit;

namespace Scraper.Service.Core.Consumers;

public class ScrapeAlbumDetailsConsumer : IConsumer<ScrapeAlbumDetails>
{
    private readonly IMusicServiceClient _client;
    
    public ScrapeAlbumDetailsConsumer(IMusicServiceClient client)
    {
        _client = client;
    }
    
    public async Task Consume(ConsumeContext<ScrapeAlbumDetails> context)
    {
        var albumId = context.Message.AlbumId;
        var album = await _client.GetAlbumInfo(albumId);

        if (album == null)
        {
            await context.Publish(new ScrapingFailed(context.Message.CorrelationId,
                $"Cannot scrap data for album with id: {albumId}"));
        }
        else
        {
            await context.Publish(new SaveAlbumData(context.Message.CorrelationId, album));   
        }
    }
}