using Catalog.Service.Core.Dto;
using Catalog.Service.Core.Models;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Errors;

namespace Catalog.Service.Core.Services;

public class CatalogService(CatalogDbContext db, ILogger<CatalogService> logger) : ICatalogService
{
    public async Task<List<AlbumDto>> GetAlbumsByArtistId(string artistId)
    {
        logger.LogInformation("Fetching albums for artist {ArtistId}", artistId);
        
        return await db.Albums
            .AsNoTracking()
            .Where(a => a.ArtistId == artistId)
            .Select(a => new AlbumDto(
                a.Id ?? string.Empty, a.ArtistId, a.Name, a.ReleaseDate, a.TotalTracks, a.ImageUrl)
            )
            .ToListAsync();
    }

    public async Task<List<TrackDto>> GetTracksByAlbumId(string albumId)
    {
        return await db.Tracks
            .AsNoTracking()
            .Where(t => t.AlbumId == albumId)
            .Select(t => new TrackDto(
                t.Id, t.Name, t.DurationMs, t.TrackNumber)
            )
            .ToListAsync();
    }

    public async Task<Album> Create(Album album)
    {
        if (!string.IsNullOrWhiteSpace(album.Id))
        {
            var existing = await db.Albums.FindAsync(album.Id);
            if (existing is not null)
            {
                throw new ResourceAlreadyExistsException();
            }
        }
        else
        {
            album.Id = IdGenerator.Generate();
        }

        db.Albums.Add(album);
        await db.SaveChangesAsync();

        return album;
    }

    public async Task<Album?> Update(Album album)
    {
        if (string.IsNullOrWhiteSpace(album.Id))
        {
            throw new Exception("Album ID is required for update.");
        }

        var existingAlbum = await db.Albums
            .Include(a => a.Tracks)
            .FirstOrDefaultAsync(a => a.Id == album.Id);
        if (existingAlbum is null)
        {
            return null;
        }

        existingAlbum.Name = album.Name;
        existingAlbum.ReleaseDate = album.ReleaseDate;
        existingAlbum.TotalTracks = album.TotalTracks;
        existingAlbum.ImageUrl = album.ImageUrl;
        
        album.Tracks.ForEach(track =>
        {
            if (track.Id == Guid.Empty)
            {
                var existingTrack = existingAlbum.Tracks.FirstOrDefault(t => t.Id == track.Id);
                if (existingTrack != null)
                {
                    existingTrack.Name = track.Name;
                    existingTrack.DurationMs = track.DurationMs;
                    existingTrack.TrackNumber = track.TrackNumber;
                }
                else
                {
                    throw new Exception("Invalid track ID");
                }
            }
            else
            {
                existingAlbum.Tracks.Add(new Track
                {
                    Name = track.Name,
                    AlbumId = track.AlbumId,
                    DurationMs = track.DurationMs,
                    TrackNumber = track.TrackNumber
                });
            }
        });

        var tracksToRemove = existingAlbum.Tracks
            .Where(t => album.Tracks.All(td => td.Id == t.Id))
            .ToList();
        
        db.Tracks.RemoveRange(tracksToRemove);

        await db.SaveChangesAsync();
        return existingAlbum;
    }

    public async Task<bool> Delete(string albumId)
    {
        var album = await db.Albums.FindAsync(albumId);
        if (album is null)
        {
            return false;
        }

        db.Albums.Remove(album);

        var deletedCount = await db.SaveChangesAsync();
        return deletedCount > 0;
    }

    public async Task DeleteAlbumsByArtistId(string artistId)
    {
        await db.Albums
            .Where(a => a.ArtistId == artistId)
            .ExecuteDeleteAsync();
    }
}