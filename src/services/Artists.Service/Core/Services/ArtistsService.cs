using Artists.Service.Core.Dto;
using Artists.Service.Core.Models;
using Artists.Service.Core.Repositories;

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
        // todo handle id generation
        return await repository.Create(artistDto);
    }
    
    public async Task<Artist?> Update(string id, ArtistDto artist)
    {
        return await repository.Update(id, artist);
    }
    
    public async Task<bool> Delete(string id)
    {
        return await repository.Delete(id);
    }
}