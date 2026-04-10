namespace Catalog.Service.Core.Models;

public class Track
{
    public Guid Id { get; set; }
    
    public required string AlbumId { get; set; }
    public Album Album { get; set; }
    
    public required string Name { get; set; }
    public int DurationMs { get; set; }
    public int TrackNumber { get; set; }
}