namespace Artists.Service.Core.Dto;

public class ArtistBaseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}