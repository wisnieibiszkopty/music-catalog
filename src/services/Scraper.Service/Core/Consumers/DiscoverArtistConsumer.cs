using MassTransit;
using Contracts;
using Scraper.Service.Core.MusicServiceClient;

namespace Scraper.Service.Core.Consumers;

public class DiscoverArtistConsumer : IConsumer<DiscoverArtist>
{
    private readonly IMusicServiceClient _client;

    public DiscoverArtistConsumer(IMusicServiceClient client)
    {
        _client = client;
    }
    
    public async Task Consume(ConsumeContext<DiscoverArtist> context)
    {
        var artistName = context.Message.ArtistName;
        var artist = await _client.GetArtistByName(artistName);

        Console.WriteLine(artist.Id);
        Console.WriteLine(artist.Name);
        Console.WriteLine(artist.ImageUrl);
        
        await context.Publish(new SaveArtistData(artist));
    }
}