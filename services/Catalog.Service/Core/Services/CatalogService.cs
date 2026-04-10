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
                logger.LogWarning("Conflict: Album with ID {AlbumId} already exists", album.Id);
                throw new ResourceAlreadyExistsException();
            }
        }
        else
        {
            album.Id = IdGenerator.Generate();
        }

        db.Albums.Add(album);
        await db.SaveChangesAsync();

        logger.LogInformation("Successfully created album {AlbumId}", album.Id);
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
            logger.LogWarning("Update aborted: Album {AlbumId} not found", album.Id);
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
                    logger.LogError("Invalid track reference {TrackId} for album {AlbumId}", track.Id, album.Id);
                    throw new Exception("Invalid track ID");
                }
            }
            else
            {
                logger.LogDebug("Adding new track {TrackName} to album {AlbumId}", track.Name, album.Id);
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
        logger.LogInformation("Removing {TrackCount} tracks from album {AlbumId}", tracksToRemove.Count, album.Id);
        
        return existingAlbum;
    }

    public async Task<bool> Delete(string albumId)
    {
        var album = await db.Albums.FindAsync(albumId);
        if (album is null)
        {
            logger.LogWarning("Delete failed: Album {AlbumId} does not exist", albumId);
            return false;
        }

        db.Albums.Remove(album);

        var affectedRows = await db.SaveChangesAsync();
        
        var success = affectedRows > 0;
        if (success)
        {
            logger.LogInformation("Successfully deleted album {AlbumId}", albumId);
        }

        return success;
    }

    public async Task DeleteAlbumsByArtistId(string artistId)
    {
        logger.LogWarning("Bulk delete requested for artist {ArtistId}", artistId);
        
        await db.Albums
            .Where(a => a.ArtistId == artistId)
            .ExecuteDeleteAsync();
    }
}