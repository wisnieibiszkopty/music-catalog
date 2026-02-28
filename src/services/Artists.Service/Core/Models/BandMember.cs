namespace Artists.Service.Core.Models;

public partial class BandMember
{
    public Guid MusicianId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Role { get; set; }
}