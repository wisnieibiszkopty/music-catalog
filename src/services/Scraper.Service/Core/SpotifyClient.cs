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

    public async Task<string> GetAlbumsByArtistId(string artistId)
    {
        await SetBearerToken();

        // generalnie bedzie trzeba po kolei leciec requesty az sie skoncza albumy, bo dziady dały limity xDDDD 
        // na początku tyl dobrze ża daje jaki jest total
        var response = await _http.GetAsync($"artists/{Uri.EscapeDataString(artistId)}/albums?include_groups=album&limit=10");
        
        var body = await response.Content.ReadAsStringAsync();
        
        _logger.LogInformation(body);
        
        return body;
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