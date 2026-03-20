namespace Catalog.Service.Core.Dto;

public record AlbumDto(
    string Id,
    string ArtistId,
    string Name,
    string ReleaseDate,
    int TotalTracks,
    string? ImageUrl
);