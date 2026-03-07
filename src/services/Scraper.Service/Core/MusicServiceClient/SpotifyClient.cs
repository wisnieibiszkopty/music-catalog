using System.Net.Http.Headers;
using Contracts;

namespace Scraper.Service.Core;

public class SpotifyClient : IMusicServiceClient
{
    private readonly ILogger _logger;
    private readonly HttpClient _http;
    private readonly BearerTokenProvider _bearerTokenProvider;

    public SpotifyClient(
        ILogger<SpotifyClient> logger,
        HttpClient httpClient,
        BearerTokenProvider bearerTokenProvider)
    {
        _logger = logger;
        _http = httpClient;
        _bearerTokenProvider = bearerTokenProvider;
    }

    private async Task SetBearerToken()
    {
        var token = await _bearerTokenProvider.GetTokenAsync();
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    // TODO format data and push to queue
    public async Task<ArtistDetails> GetArtistByName(string name)
    {
        await SetBearerToken();

        var artistId = await GetArtistId(name);
        var artistDetails = await GetArtistDetails(artistId!);
        
        return artistDetails;
    }

    private async Task<string?> GetArtistId(string artistName)
    {
        var url = $"search?q={Uri.EscapeDataString(artistName)}&type=artist&limit=1";
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var body = await response.Content.ReadAsStringAsync();

        var artistScraper = new ArtistExtractor();
        var artistId = artistScraper.ExtractArtistId(body);
        return artistId;
    }

    private async Task<ArtistDetails> GetArtistDetails(string artistId)
    {
        var url = $"artists/{Uri.EscapeDataString(artistId)}";
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var body = await response.Content.ReadAsStringAsync();
        
        var artistScraper = new ArtistExtractor();
        var artistDetails = artistScraper.ExtractArtistDetails(body);
        return artistDetails;
    }
    
    public async Task<List<string>> GetAlbumsByArtistId(string artistId)
    {
        await SetBearerToken();
        
        var allAlbumIds = new List<string>();
        int offset = 0;
        const int limit = 10;
        int total = 0;

        do
        {
            var url = $"artists/{Uri.EscapeDataString(artistId)}/albums?include_groups=album&limit={limit}&offset={offset}";
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var scraper = new AlbumExtractor();
            var (ids, apiTotal) = scraper.ParseAlbumsPage(body);
        
            allAlbumIds.AddRange(ids);
            total = apiTotal; 
            offset += limit; 

            _logger.LogInformation("Fetched {Count}/{Total} albums for ArtistId {ArtistId}", allAlbumIds.Count, total, artistId);

        } while (allAlbumIds.Count < total);

        return allAlbumIds;
    }

    public async Task<AlbumDetails?> GetAlbumInfo(string albumId)
    {
        await SetBearerToken();

        var url = $"albums/{Uri.EscapeDataString(albumId)}";
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var body = await response.Content.ReadAsStringAsync();

        var scraper = new Core.AlbumExtractor();
        var albumDetails = scraper.ExtractFullAlbumInfo(body);
        
        _logger.LogInformation("Fetched album details for album with id {AlbumId}", albumId);
        
        return albumDetails;
    }
}