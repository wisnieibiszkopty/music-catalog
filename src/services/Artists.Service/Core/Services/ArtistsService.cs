using Artists.Service.Core.Dto;
using Artists.Service.Core.Models;
using Artists.Service.Core.Repositories;
using Shared;
using Shared.Errors;

namespace Artists.Service.Core.Services;

public class ArtistsService(IArtistsRepository repository): IArtistsService
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
        
        return await repository.Create(artistDto);
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
            return null;
        }
        
        return await repository.Update(artistDto);
    }
    
    public async Task<bool> Delete(string id)
    {
        return await repository.Delete(id);
    }
}