using MassTransit;
using Contracts;
using Scraper.Service.Core.MusicServiceClient;

namespace Scraper.Service.Core.Consumers;

public class DiscoverArtistConsumer : IConsumer<DiscoverArtist>
{
    private readonly ILogger<DiscoverArtistConsumer> _logger;
    private readonly IMusicServiceClient _client;

    public DiscoverArtistConsumer(IMusicServiceClient client, ILogger<DiscoverArtistConsumer> logger)
    {
        _logger = logger;
        _client = client;
    }
    
    public async Task Consume(ConsumeContext<DiscoverArtist> context)
    {
        var artistName = context.Message.ArtistName;
        var artist = await _client.GetArtistByName(artistName);
        
        _logger.LogInformation("Consumed DiscoverArtist event. ArtistId: {ArtistId}", artist.Id);
        
        await context.Publish(new SaveArtistData(artist));
    }
}