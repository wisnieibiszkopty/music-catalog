using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Scraper.Service.Core.MusicServiceClient;

public class BearerTokenProvider
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IDistributedCache _cache;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _tokenAddress;
    
    public BearerTokenProvider(
        IHttpClientFactory httpFactory,
        IDistributedCache cache,
        IConfiguration config)
    {
        _httpFactory = httpFactory;
        _cache = cache;
        _clientId = config["Spotify:ClientId"]!;
        _clientSecret = config["Spotify:ClientSecret"]!;
        _tokenAddress = config["MusicService:TokenAddress"]!;
    }

    public async Task<string> GetTokenAsync()
    {
        var cached = await _cache.GetStringAsync("spotify:token");
        if (!string.IsNullOrEmpty(cached))
            return cached;

        var http = _httpFactory.CreateClient();

        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _clientId,
            ["client_secret"] = _clientSecret
        });

        var response = await http.PostAsync(_tokenAddress, body);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        using var json = await JsonDocument.ParseAsync(stream);

        var accessToken = json.RootElement.GetProperty("access_token").GetString()!;
        var expiresIn = json.RootElement.GetProperty("expires_in").GetInt32();
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiresIn - 60)
        };

        await _cache.SetStringAsync("spotify:token", accessToken, options);

        return accessToken;
    }
}