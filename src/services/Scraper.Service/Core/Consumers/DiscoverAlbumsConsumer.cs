using Contracts;
using MassTransit;

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
        var albums = await _client.GetAlbumsByArtistId(artistId);
        
        albums.ForEach(album => 
            context.Publish(new AlbumsDiscovered(context.Message.CorrelationId, albums))
        );
    }
}