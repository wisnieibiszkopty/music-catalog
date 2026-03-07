namespace Contracts;

public record StartAlbumsScraping(Guid CorrelationId, string ArtistId);
public record DiscoverAlbums(Guid CorrelationId, string ArtistId);
public record AlbumsDiscovered(Guid CorrelationId, List<string> AlbumIds);
public record ScrapeAlbumDetails(Guid CorrelationId, string AlbumId);
public record SaveAlbumData(Guid CorrelationId, string AlbumId, string AlbumName, List<string> Songs);
public record AlbumSaved(Guid CorrelationId, string AlbumId);
public record AllAlbumsScraped(Guid CorrelationId, string ArtistId);
public record ScrapingFailed(Guid CorrelationId, string ErrorMessage);