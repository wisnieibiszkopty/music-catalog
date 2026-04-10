namespace Artists.Service.Core.Dto;

public class ArtistBaseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}