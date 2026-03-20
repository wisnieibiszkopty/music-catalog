namespace Catalog.Service.Core.Models;

public class Album
{
    public string Id { get; set; }
    public string ArtistId { get; set; }
    public string Name { get; set; }
    public string ReleaseDate { get; set; }
    public int TotalTracks { get; set; }
    public string? ImageUrl { get; set; }
    
    public List<Track> Tracks { get; set; } = new();
}