namespace Catalog.Service.Core.Dto;

public record TrackDto(
    Guid Id,
    string Name,
    int DurationMs,
    int TrackNumber
);