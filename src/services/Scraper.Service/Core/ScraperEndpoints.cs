namespace Scraper.Service.Core;

public static class ScrapperEndpoints
{
    public static IEndpointRouteBuilder MapScrapperEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/scrapper")
            .WithTags("Scrapper");

        group.MapGet("/artist/{artistName}", ScrapArtistByName);
        group.MapGet("/albums/{artistId}", ScrapArtistsAlbums);
        group.MapGet("/album-info/{albumId}", ScrapAlbum);
        
        return builder;
    }

    private static async Task<IResult> ScrapArtistByName(string artistName, IMusicServiceClient client)
    {
        var result = await client.GetArtistByName(artistName);
        return Results.Ok(result);
    }

    private static async Task<IResult> ScrapArtistsAlbums(string artistId, IMusicServiceClient client)
    {
        var result = await client.GetAlbumsByArtistId(artistId);
        return Results.Ok(result);
    }

    private static async Task<IResult> ScrapAlbum(string albumId, IMusicServiceClient client)
    {
        var result = await client.GetAlbumInfo(albumId);
        return Results.Ok(result);
    }
}