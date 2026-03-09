using Contracts;
using MassTransit;
using Shared.Auth;

namespace Orchestrator.Service;

public static class ScrappingEndpoints
{
    public static IEndpointRouteBuilder MapScrappingEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/scraper")
            .WithTags("Scrapper");

        group.MapPost("/artists/{artistName}", ScrapArtistByName).RequireAuthorization(Policies.Admin);
        group.MapPost("/albums/{artistId}", ScrapArtistsAlbums).RequireAuthorization(Policies.Admin);
        
        return builder;
    }
    
    private static async Task<IResult> ScrapArtistByName(string artistName, IPublishEndpoint publishEndpoint)
    {
        await publishEndpoint.Publish(new DiscoverArtist(artistName));
        return Results.Accepted();
    }

    private static async Task<IResult> ScrapArtistsAlbums(string artistId, IPublishEndpoint publishEndpoint)
    {
        var correlationId = Guid.NewGuid();
        await publishEndpoint.Publish(new StartAlbumsScraping(correlationId, artistId));
        return Results.Accepted();
    }
}