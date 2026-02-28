namespace Artist.Service.Core;

public interface IArtistService
{
    Task<IEnumerable<Artist>> GetAll();
    Task<Artist> GetById(int id);
    Task<Artist> Create(Artist artist);
}