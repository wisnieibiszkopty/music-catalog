namespace Catalog.Service.Core.Dto;

public record TrackDto(
    int Id,
    string Name,
    int DurationMs,
    int TrackNumber
);