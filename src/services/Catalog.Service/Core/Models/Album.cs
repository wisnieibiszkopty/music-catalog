using Contracts;

namespace Catalog.Service.Core.Models;

public class Album
{
    public string? Id { get; set; }
    public string ArtistId { get; set; }
    public string Name { get; set; }
    public string ReleaseDate { get; set; }
    public int TotalTracks { get; set; }
    public string? ImageUrl { get; set; }
    
    public List<Track> Tracks { get; set; } = new();

    public Album() {}
    
    public Album(AlbumDetails albumDetails)
    {
        Id = albumDetails.Id;
        ArtistId = albumDetails.ArtistId;
        Name = albumDetails.Name;
        ReleaseDate = albumDetails.ReleaseDate;
        TotalTracks = albumDetails.TotalTracks;
        ImageUrl = albumDetails.ImageUrl;
        Tracks = albumDetails.Tracks
            .Select(t => new Track
            {
                AlbumId = albumDetails.Id,
                Name = t.Name,
                DurationMs = t.DurationMs,
                TrackNumber = t.TrackNumber
            }).ToList();
    }
}