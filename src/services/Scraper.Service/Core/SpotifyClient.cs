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
    
    public async Task<string> GetArtistByNameAsync(string name)
    {
        await SetBearerToken();
        
        var response = await _http.GetAsync($"search?q={Uri.EscapeDataString(name)}&type=artist&limit=1");
        
        var body = await response.Content.ReadAsStringAsync();
        
        _logger.LogInformation(body);
        
        return body;
    }
}