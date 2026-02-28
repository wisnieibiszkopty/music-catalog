namespace Artists.Service.Core.Models;

public partial class Artist
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? FoundedYear { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsBand { get; set; }
    
    public List<Genre> Genres { get; set; } = [];
    public List<BandMember> Members { get; set; } = [];
}