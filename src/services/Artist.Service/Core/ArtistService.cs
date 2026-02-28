using Dapper;

namespace Artist.Service.Core;

public class ArtistService(IDbConnectionFactory db): IArtistService
{
    public async Task<IEnumerable<Artist>> GetAll()
    {
        using var connection = await db.CreateConnectionAsync();
        
        var artists = await connection.QueryAsync<Artist>("SELECT * FROM artists");
    
        return artists;
    }

    public Task<Artist> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Artist> Create(Artist artist)
    {
        throw new NotImplementedException();
    }
}