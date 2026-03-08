using Artists.Service.Core.Dto;
using Artists.Service.Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace Artists.Service.Core;

public static class ArtistsEndpoint
{
    public static IEndpointRouteBuilder MapArtistEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/artists")
            .WithTags("Artists");

        group.MapGet("/", GetAll);
        group.MapGet("/{id}", GetById);
        group.MapPost("/", Create).RequireAuthorization(new AuthorizeAttribute { Roles = "admin"});
        group.MapPut("/{id}", Update).RequireAuthorization(new AuthorizeAttribute { Roles = "admin"});
        group.MapDelete("/{id}", Delete).RequireAuthorization(new AuthorizeAttribute { Roles = "admin"});
        
        return builder;
    }

    private static async Task<IResult> GetAll(IArtistsService artistsService)
    {
        var artists = await artistsService.GetAll();
        return Results.Ok(artists);
    }

    private static async Task<IResult> GetById(string id, IArtistsService artistsService)
    {
        var artist = await artistsService.GetById(id);
        return artist is not null ? Results.Ok(artist) : Results.NotFound();
    }
    
    private static async Task<IResult> Create(ArtistDto artistDto, IArtistsService artistsService, IValidator<ArtistDto> validator)
    {
        var validationResult = await validator.ValidateAsync(artistDto);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var createdArtist = await artistsService.Create(artistDto);
        return Results.Ok(createdArtist);
    }
    
    private static async Task<IResult> Update(string id, ArtistDto artistDto, IArtistsService artistsService, IValidator<ArtistDto> validator)
    {
        var validationResult = await validator.ValidateAsync(artistDto);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        
        var updatedArtist = await artistsService.Update(id, artistDto);
        return updatedArtist is not null ? Results.Ok(updatedArtist) : Results.NotFound();
    }
    
    private static async Task<IResult> Delete(string id, IArtistsService artistsService)
    {
        var deleted = await artistsService.Delete(id);
        return deleted ? Results.NoContent() : Results.BadRequest();
    }
}