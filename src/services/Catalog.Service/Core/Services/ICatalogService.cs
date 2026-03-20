using Catalog.Service.Core.Dto;
using Catalog.Service.Core.Models;

namespace Catalog.Service.Core.Services;

public interface ICatalogService
{
    public Task<List<AlbumDto>> GetAlbumsByArtistId(string artistId);
    public Task<List<TrackDto>> GetTracksByAlbumId(string albumId);
    public Task<Album> Create(Album album);
    public Task<Album?> Update(Album album);
    public Task<bool> Delete(string albumId);
}