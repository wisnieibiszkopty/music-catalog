namespace Artist.Service.Core;

public static class ArtistEndpoint
{
    public static IEndpointRouteBuilder MapArtistEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/v1/artists")
            .WithTags("Artists");

        group.MapGet("/", GetAll);

        return builder;
    }

    private static async Task<IResult> GetAll(IArtistService artistService)
    {
        var artists = await artistService.GetAll();
        return Results.Ok(artists);
    }

    private static async Task<IResult> GetById(IArtistService artistService)
    {
        return Results.Ok();
    }
    
    private static async Task<IResult> Create(IArtistService artistService)
    {
        return Results.Ok();
    }
    
    private static async Task<IResult> Update(IArtistService artistService)
    {
        return Results.Ok();
    }
    
    private static async Task<IResult> Delete(IArtistService artistService)
    {
        return Results.Ok();
    }
}