using Artists.Service.Core.Dto;
using Artists.Service.Core.Models;
using Artists.Service.Core.Repositories;
using Contracts;
using MassTransit;
using Shared;
using Shared.Errors;

namespace Artists.Service.Core.Services;

public class ArtistsService(
    ILogger<ArtistsService> logger,
    IArtistsRepository repository,
    IPublishEndpoint publishEndpoint
    ): IArtistsService 
{
    public async Task<IEnumerable<ArtistBaseDto>> GetAll()
    {
        return await repository.GetAll();
    }

    public async Task<Artist?> GetById(string id)
    {
        return await repository.GetById(id);
    }

    public async Task<Artist> Create(ArtistDto artistDto)
    {
        if (!string.IsNullOrWhiteSpace(artistDto.Id))
        {
            var existing = await repository.GetById(artistDto.Id);
            if (existing is not null)
            {
                throw new ResourceAlreadyExistsException();
            }
        }
        else
        {
            artistDto.Id = IdGenerator.Generate();
        }
        
        var createdArtist = await repository.Create(artistDto);
        logger.LogInformation("Created artist with Id: {Id}", createdArtist.Id);
        return createdArtist;
    }
    
    public async Task<Artist?> Update(ArtistDto artistDto)
    {
        if (string.IsNullOrWhiteSpace(artistDto.Id))
        {
            throw new Exception("Artist ID is required for update.");
        }
        
        var existing = await repository.GetById(artistDto.Id);
        if (existing is null)
        {
            logger.LogError("Failure during artist update. Artist with Id: {Id} doesn't exist", artistDto.Id);
            return null;
        }
        
        var updatedArtist = await repository.Update(artistDto);
        logger.LogInformation("Updated artist with id: {Id}", updatedArtist.Id);
        return updatedArtist;
    }
    
    public async Task<bool> Delete(string id)
    {
        var deleted = await repository.Delete(id);

        if (deleted)
        {
            logger.LogInformation("Deleted artist with Id: {Id}", id);
            await publishEndpoint.Publish(new ArtistDeleted(id));   
        }
        else
        {
            logger.LogError("Cannot delete artist with Id: {Id}", id);
        }
        
        return deleted;
    }
}