using Contracts;

namespace Scraper.Service.Core;

public interface IMusicServiceClient
{
    public Task<ArtistDetails> GetArtistByName(string name);
    public Task<List<string>> GetAlbumsByArtistId(string artistId);
    public Task<AlbumDetails?> GetAlbumInfo(string albumId);
}