using Artists.Service.Core.Models;

namespace Artists.Service.Core;

public interface IArtistService
{
    Task<IEnumerable<Artist>> GetAll();
    Task<Artist> GetById(Guid id);
    Task<Artist> Create(Artist artist);
    Task<Artist> Update(Artist artist);
    Task Delete(Guid id);
}