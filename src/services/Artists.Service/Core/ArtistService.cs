using Artists.Service.Core.Dto;
using Dapper;
using Artists.Service.Core.Models;

namespace Artists.Service.Core;

public class ArtistService(IDbConnectionFactory db): IArtistService
{
    public async Task<IEnumerable<ArtistBaseDto>> GetAll()
    {
        using var connection = await db.CreateConnectionAsync();
        
        string sql = "SELECT id, name, image_url AS ImageUrl FROM artists";
        var artists = await connection.QueryAsync<ArtistBaseDto>(sql);
    
        return artists;
    }

    public async Task<Artist?> GetById(Guid id)
    {
        using var connection = await db.CreateConnectionAsync();

        const string sql = "SELECT * FROM artists WHERE id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Artist>(sql, new { Id = id });
    }

    public async Task<Artist> Create(ArtistDto artistDto)
    {
        using var connection = await db.CreateConnectionAsync();

        const string sql = @"
            INSERT INTO artists (name, founded_year, description, image_url, is_band)
            VALUES (@Name, @FoundedYear, @Description, @ImageUrl, @IsBand)
            RETURNING *;"; 
        
        var createdArtist = await connection.QuerySingleAsync<Artist>(sql, artistDto);
    
        return createdArtist;
    }

    // TODO id is broken
    public async Task<Artist?> Update(Guid id, ArtistDto artist)
    {
        using var connection = await db.CreateConnectionAsync();

        const string sql = @"
            UPDATE artists 
            SET name = @Name, 
                founded_year = @FoundedYear, 
                description = @Description, 
                image_url = @ImageUrl, 
                is_band = @IsBand
            WHERE id = @Id
            RETURNING *;";
        
        return await connection.QueryFirstOrDefaultAsync<Artist>(sql, artist);
    }

    // TODO id is broken
    public async Task<bool> Delete(Guid id)
    {
        using var connection = await db.CreateConnectionAsync();

        const string sql = "DELETE FROM artists WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}