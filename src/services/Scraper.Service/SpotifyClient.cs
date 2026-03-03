using System.Net.Http.Headers;
using Contracts;

namespace Scraper.Service;

public class SpotifyClient
{
    private readonly HttpClient _http;

    public SpotifyClient(HttpClient httpClient)
    {
        _http = httpClient;
    }

    public async Task<SearchArtist?> GetArtistAsync(string name, string token)
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var response = await _http.GetAsync($"search?q={Uri.EscapeDataString(name)}&type=artist");
        return await response.Content.ReadFromJsonAsync<SearchArtist>();
    }
}