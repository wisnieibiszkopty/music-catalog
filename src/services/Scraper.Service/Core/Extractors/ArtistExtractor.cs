using System.Text.Json;
using Contracts;

namespace Scraper.Service.Core.Extractors;

public class ArtistExtractor
{
    public string? ExtractArtistId(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        
        if (root.TryGetProperty("artists", out var artists) &&
            artists.TryGetProperty("items", out var items) &&
            items.GetArrayLength() > 0)
        {
            var firstArtist = items[0];
        
            if (firstArtist.TryGetProperty("id", out var idProp))
            {
                return idProp.GetString();
            }
        }

        return null;
    }

    public ArtistDetails ExtractArtistDetails(string json)
    {
        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            JsonElement root = doc.RootElement;

            return new ArtistDetails(
                root.GetProperty("id").GetString() ?? string.Empty,
                root.GetProperty("name").GetString() ?? string.Empty,
                root.GetProperty("images")[0].GetProperty("url").GetString() ?? string.Empty  
            );
        }
    }
}