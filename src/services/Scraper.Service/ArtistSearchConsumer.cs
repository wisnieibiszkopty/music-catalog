using MassTransit;
using Contracts;

namespace Scraper.Service;

public class ArtistSearchConsumer : IConsumer<SearchArtist>
{
    private readonly SpotifyClient _spotifyClient;

    public ArtistSearchConsumer(SpotifyClient spotifyClient)
    {
        _spotifyClient = spotifyClient;
    }
    
    public async Task Consume(ConsumeContext<SearchArtist> context)
    {
        Console.WriteLine(context.Message.Name);

        await Task.CompletedTask;
    }
}