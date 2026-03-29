using Catalog.Service.Core.Models;
using Catalog.Service.Core.Services;
using FluentValidation;
using Shared.Auth;

namespace Catalog.Service.Core;

public static class CatalogEndpoint
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/catalog")
            .WithTags("Catalog");

        group.MapGet("/albums/{artistId}", GetAlbumsByArtistId);
        group.MapGet("/albums/songs/{albumId}", GetTracksByAlbumId);
        group.MapPost("/", Create).RequireAuthorization(Policies.Admin);
        group.MapPut("/", Update).RequireAuthorization(Policies.Admin);
        group.MapDelete("/{albumId}", Delete).RequireAuthorization(Policies.Admin);
        
        return builder;
    }

    private static async Task<IResult> GetAlbumsByArtistId(string artistId, ICatalogService catalogService)
    {
        var albums = await catalogService.GetAlbumsByArtistId(artistId);
        return Results.Ok(albums);
    }

    private static async Task<IResult> GetTracksByAlbumId(string albumId, ICatalogService catalogService)
    {
        var tracks = await catalogService.GetTracksByAlbumId(albumId);
        return Results.Ok(tracks);
    }

    private static async Task<IResult> Create(Album album, ICatalogService catalogService, IValidator<Album> validator)
    {
        var validationResult = await validator.ValidateAsync(album);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var createdAlbum = catalogService.Create(album);
        return Results.Ok(createdAlbum);
    }

    private static async Task<IResult> Update(Album album, ICatalogService catalogService, IValidator<Album> validator)
    {
        var validationResult = await validator.ValidateAsync(album);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var updatedAlbum = await catalogService.Update(album);
        return updatedAlbum is not null ? Results.Ok(updatedAlbum) : Results.NotFound();
    }

    private static async Task<IResult> Delete(string albumId, ICatalogService catalogService)
    {
        var deleted = await catalogService.Delete(albumId);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}