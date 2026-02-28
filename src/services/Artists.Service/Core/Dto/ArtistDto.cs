namespace Artists.Service.Core.Dto;

public class ArtistDto
{
    public string Name { get; set; } = string.Empty;
    public int? FoundedYear { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsBand { get; set; }
}