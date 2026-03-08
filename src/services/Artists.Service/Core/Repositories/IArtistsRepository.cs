using Artists.Service.Core.Dto;
using Artists.Service.Core.Models;

namespace Artists.Service.Core.Repositories;

public interface IArtistsRepository
{
    Task<IEnumerable<ArtistBaseDto>> GetAll();
    Task<Artist?> GetById(string id);
    Task<Artist> Create(ArtistDto artistDto);
    Task<Artist?> Update(string id, ArtistDto artist);
    Task<bool> Delete(string id);
}