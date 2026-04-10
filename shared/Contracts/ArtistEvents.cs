namespace Contracts;

public record DiscoverArtist(string ArtistName);
public record SaveArtistData(ArtistDetails Artist);
public record ArtistSaved(ArtistDetails Artist);

public record ArtistDeleted(string ArtistId);