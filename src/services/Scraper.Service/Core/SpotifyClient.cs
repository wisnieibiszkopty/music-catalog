using System.Net.Http.Headers;
using Contracts;
using Scraper.Service.Core;

namespace Scraper.Service;

public class SpotifyClient
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
    public async Task<string> GetArtistByNameAsync(string name)
    {
        await SetBearerToken();
        
        var response = await _http.GetAsync($"search?q={Uri.EscapeDataString(name)}&type=artist&limit=1");
        
        var body = await response.Content.ReadAsStringAsync();
        
        _logger.LogInformation(body);
        
        return body;
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

            var scraper = new AlbumScraper();
            var (ids, apiTotal) = scraper.ParseAlbumsPage(body);
        
            allAlbumIds.AddRange(ids);
            total = apiTotal; 
            offset += limit; 

            _logger.LogInformation("Fetched {Count}/{Total} albums for ArtistId {ArtistId}", allAlbumIds.Count, total, artistId);

        } while (allAlbumIds.Count < total);

        return allAlbumIds;
    }

    public async Task<string> GetAlbumInfo(string albumId)
    {
        await SetBearerToken();
        
        var response = await _http.GetAsync($"albums/{Uri.EscapeDataString(albumId)}");
        
        var body = await response.Content.ReadAsStringAsync();
        
        _logger.LogInformation(body);
        
        return body;
    }
}