using Artists.Service.Core.Dto;
using Artists.Service.Core.Services;
using Contracts;
using MassTransit;

namespace Artists.Service.Core.Consumers;

public class SaveArtistDataConsumer(IArtistsService artistsService) : IConsumer<SaveArtistData>
{
    public async Task Consume(ConsumeContext<SaveArtistData> context)
    {
        var artistDetails = context.Message.Artist;
        var artistDto = new ArtistDto
        {
            Id = artistDetails.Id,
            Name = artistDetails.Name,
            ImageUrl = artistDetails.ImageUrl
        };
        
        var savedArtist = await artistsService.Create(artistDto);
        
        await context.Publish(new ArtistSaved(artistDetails));
    }
}