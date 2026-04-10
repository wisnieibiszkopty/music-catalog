using System.Net.Http.Headers;
using Contracts;
using Scraper.Service.Core.Extractors;
using Scraper.Service.Core.MusicServiceClient;

namespace Scraper.Service.Core.MusicServiceClient;

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
        try
        {
            var token = await _bearerTokenProvider.GetTokenAsync();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to retrieve Spotify Bearer Token. Authentication is impossible.");
            throw;
        }
    }
    
    public async Task<ArtistDetails> GetArtistByName(string name)
    {
        try
        {
            await SetBearerToken();
            var artistId = await GetArtistId(name);

            if (string.IsNullOrEmpty(artistId))
            {
                _logger.LogWarning("Artist search returned no results for name: {ArtistName}", name);
                throw new KeyNotFoundException($"Artist '{name}' not found on Spotify.");
            }

            return await GetArtistDetails(artistId);
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Unexpected error while fetching artist by name: {ArtistName}", name);
            throw;
        }
    }

    private async Task<string?> GetArtistId(string artistName)
    {
        var url = $"search?q={Uri.EscapeDataString(artistName)}&type=artist&limit=1";
        
        _logger.LogDebug("Calling Spotify Search API for: {ArtistName}", artistName);
        var response = await _http.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Spotify Search API failed. Status: {StatusCode}, Artist: {ArtistName}", 
                (int)response.StatusCode, artistName);
        }
        
        response.EnsureSuccessStatusCode();
        
        var body = await response.Content.ReadAsStringAsync();
        var artistId = new ArtistExtractor().ExtractArtistId(body);
        
        return artistId;
    }

    private async Task<ArtistDetails> GetArtistDetails(string artistId)
    {
        var url = $"artists/{Uri.EscapeDataString(artistId)}";
        
        _logger.LogDebug("Calling Spotify Artist Details API. ArtistId: {ArtistId}", artistId);
        var response = await _http.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Spotify Artist Details API failed. Status: {StatusCode}, ArtistId: {ArtistId}", 
                (int)response.StatusCode, artistId);
        }
        
        response.EnsureSuccessStatusCode();
        
        var body = await response.Content.ReadAsStringAsync();
        var artistDetails = new ArtistExtractor().ExtractArtistDetails(body);
        
        return artistDetails;
    }
    
    public async Task<List<string>> GetAlbumsByArtistId(string artistId)
    {
        await SetBearerToken();
        
        var allAlbumIds = new List<string>();
        int offset = 0;
        const int limit = 10;
        int total = 0;
        
        _logger.LogInformation("Starting to fetch albums for ArtistId: {ArtistId}", artistId);
        
        do
        {
            var url = $"artists/{Uri.EscapeDataString(artistId)}/albums?include_groups=album&limit={limit}&offset={offset}";
            var response = await _http.GetAsync(url);

            if ((int)response.StatusCode == 429)
            {
                var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds
                                 ?? (response.Headers.RetryAfter?.Date - DateTimeOffset.UtcNow)?.TotalSeconds
                                 ?? 1;

                _logger.LogCritical("Rate limited. Retry-After: {Seconds} seconds", retryAfter);
                throw new MusicServiceRateLimitException(retryAfter);
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Spotify Albums API failed. Status: {StatusCode}, ArtistId: {ArtistId}, Offset: {Offset}", 
                    (int)response.StatusCode, artistId, offset);
            }
            
            var body = await response.Content.ReadAsStringAsync();
            var extractor = new AlbumExtractor();
            var (ids, apiTotal) = extractor.ParseAlbumsPage(body);
    
            allAlbumIds.AddRange(ids);
            total = apiTotal; 
            offset += limit; 

            _logger.LogInformation("Fetched {Count}/{Total} albums for ArtistId {ArtistId}", allAlbumIds.Count, total, artistId);

        } while (allAlbumIds.Count < total);

        _logger.LogInformation("Successfully fetched all {Total} albums for ArtistId {ArtistId}", 
            allAlbumIds.Count, artistId);
        
        return allAlbumIds;
    }

    public async Task<AlbumDetails?> GetAlbumInfo(string albumId, string artistId)
    {
        await SetBearerToken();

        var url = $"albums/{Uri.EscapeDataString(albumId)}";
        
        _logger.LogDebug("Calling Spotify Album Details API. AlbumId: {AlbumId}", albumId);
        var response = await _http.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Spotify Album API failed. Status: {StatusCode}, AlbumId: {AlbumId}, ArtistId: {ArtistId}", 
                (int)response.StatusCode, albumId, artistId);
        }
        
        response.EnsureSuccessStatusCode();
        
        var body = await response.Content.ReadAsStringAsync();

        var extractor = new AlbumExtractor();
        var albumDetails = extractor.ExtractFullAlbumInfo(body, artistId);
        
        _logger.LogInformation("Successfully fetched album details. AlbumId: {AlbumId}, Name: {AlbumName}", 
            albumId, albumDetails.Name);
        
        return albumDetails;
    }
}