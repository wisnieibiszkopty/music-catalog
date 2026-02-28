using Dapper;
using Artists.Service.Core.Models;

namespace Artists.Service.Core;

public class ArtistService(IDbConnectionFactory db): IArtistService
{
    public async Task<IEnumerable<Artist>> GetAll()
    {
        using var connection = await db.CreateConnectionAsync();
        
        string sql = "SELECT id, name, image_url AS ImageUrl FROM artists";
        var artists = await connection.QueryAsync<Artist>(sql);
    
        return artists;
    }

    public async Task<Artist> GetById(Guid id)
    {
        using var connection = await db.CreateConnectionAsync();

        const string sql = "SELECT * FROM artists WHERE id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Artist>(sql, new { Id = id });
    }

    public Task<Artist> Create(Artist artist)
    {
        throw new NotImplementedException();
    }

    public Task<Artist> Update(Artist artist)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }
}