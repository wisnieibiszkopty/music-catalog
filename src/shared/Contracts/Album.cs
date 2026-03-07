namespace Contracts;

public record AlbumDetails(
    string Name,
    string ReleaseDate,
    int TotalTracks,
    string? ImageUrl,
    List<TrackInfo> Tracks
);

public record TrackInfo(
    string Name,
    int DurationMs,
    int TrackNumber
);