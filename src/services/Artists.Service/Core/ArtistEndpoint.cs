using Artists.Service.Core.Dto;
using Artists.Service.Core.Models;
using Artists.Service.Core.Validators;

namespace Artists.Service.Core;

public static class ArtistEndpoint
{
    public static IEndpointRouteBuilder MapArtistEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/artists")
            .WithTags("Artists");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:guid}", Update);
        group.MapDelete("/{id:guid}", Delete);
        
        return builder;
    }

    private static async Task<IResult> GetAll(IArtistService artistService)
    {
        var artists = await artistService.GetAll();
        return Results.Ok(artists);
    }

    private static async Task<IResult> GetById(Guid id, IArtistService artistService)
    {
        var artist = await artistService.GetById(id);
        return artist is not null ? Results.Ok(artist) : Results.NotFound();
    }
    
    private static async Task<IResult> Create(ArtistDto artistDto, IArtistService artistService)
    {
        var validator = new CreateArtistValidator();
        var validationResult = await validator.ValidateAsync(artistDto);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var createdArtist = await artistService.Create(artistDto);
        return Results.Ok(createdArtist);
    }
    
    private static async Task<IResult> Update(Guid id, ArtistDto artistDto, IArtistService artistService)
    {
        var validator = new CreateArtistValidator();
        var validationResult = await validator.ValidateAsync(artistDto);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        
        var updatedArtist = await artistService.Update(id, artistDto);
        return updatedArtist is not null ? Results.Ok(updatedArtist) : Results.NotFound();
    }
    
    private static async Task<IResult> Delete(Guid id, IArtistService artistService)
    {
        var deleted = await artistService.Delete(id);
        return deleted ? Results.NoContent() : Results.BadRequest();
    }
}