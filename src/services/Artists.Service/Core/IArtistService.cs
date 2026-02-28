using Artists.Service.Core.Dto;
using Artists.Service.Core.Models;

namespace Artists.Service.Core;

public interface IArtistService
{
    Task<IEnumerable<ArtistBaseDto>> GetAll();
    Task<Artist?> GetById(Guid id);
    Task<Artist> Create(ArtistDto artistDto);
    Task<Artist?> Update(Guid id, ArtistDto artist);
    Task<bool> Delete(Guid id);
}