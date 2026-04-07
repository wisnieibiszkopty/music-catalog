using Artists.Service.Core.Dto;
using Artists.Service.Core.Services;
using Contracts;
using MassTransit;

namespace Artists.Service.Core.Consumers;

public class SaveArtistDataConsumer(ILogger<SaveArtistDataConsumer> logger, IArtistsService artistsService) : IConsumer<SaveArtistData>
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
        logger.LogInformation(
            "Consuming SaveArtistData event. Id: {Id}, name: {Name}", savedArtist.Id, savedArtist.Name
        );
        
        await context.Publish(new ArtistSaved(artistDetails));
    }
}