namespace Scraper.Service.Core;

public static class ScrapperEndpoints
{
    public static IEndpointRouteBuilder MapScrapperEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/scrapper")
            .WithTags("Scrapper");

        group.MapGet("/artist/{artistName}", ScrapArtistByName);
        group.MapGet("/albums/{artistId}", ScrapArtistsAlbums);
        
        return builder;
    }

    private static async Task<IResult> ScrapArtistByName(string artistName, SpotifyClient client)
    {
        var result = await client.GetArtistByNameAsync(artistName);
        return Results.Content(result, "application/json");
    }

    private static async Task<IResult> ScrapArtistsAlbums(string artistId, SpotifyClient client)
    {
        var result = await client.GetAlbumsByArtistId(artistId);
        return Results.Content(result, "application/json");
    }
}