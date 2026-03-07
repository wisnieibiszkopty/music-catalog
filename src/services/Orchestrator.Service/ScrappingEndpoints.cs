using Contracts;
using MassTransit;

namespace Orchestrator.Service;

public static class ScrappingEndpoints
{
    public static IEndpointRouteBuilder MapScrappingEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/scrapper")
            .WithTags("Scrapper");

        group.MapGet("/artists/{artistName}", ScrapArtistByName);
        group.MapGet("/albums/{artistId}", ScrapArtistsAlbums);
        
        return builder;
    }
    
    private static async Task<IResult> ScrapArtistByName(string artistName)
    {
        await Task.CompletedTask;
        return Results.Ok();
    }

    private static async Task<IResult> ScrapArtistsAlbums(string artistId, IPublishEndpoint publishEndpoint)
    {
        var correlationId = Guid.NewGuid();
        await publishEndpoint.Publish(new StartAlbumsScraping(correlationId, artistId));
        return Results.Accepted($"/status/{correlationId}", new { CorrelationId = correlationId });
    }
}