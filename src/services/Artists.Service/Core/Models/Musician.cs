namespace Artists.Service.Core.Models;

public partial class Musician
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
}