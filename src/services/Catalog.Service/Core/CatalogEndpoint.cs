using Shared.Auth;

namespace Catalog.Service.Core;

public static class CatalogEndpoint
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/catalog")
            .WithTags("Catalog");

        group.MapGet("/albums/{artistId}", GetByArtistId);
        group.MapGet("/albums/songs/{albumId}", GetSongsByAlbumId);
        group.MapPost("/", Create).RequireAuthorization(Policies.Admin);
        group.MapPut("/", Update).RequireAuthorization(Policies.Admin);
        group.MapDelete("/{albumId}", Delete).RequireAuthorization(Policies.Admin);
        
        return builder;
    }

    private static async Task<IResult> GetByArtistId(string artistId)
    {
        throw new NotImplementedException();
    }

    private static async Task<IResult> GetSongsByAlbumId(string albumId)
    {
        throw new NotImplementedException();
    }

    private static async Task<IResult> Create()
    {
        throw new NotImplementedException();
    }

    private static async Task<IResult> Update()
    {
        throw new NotImplementedException();
    }

    private static async Task<IResult> Delete(string albumId)
    {
        throw new NotImplementedException();
    }
}