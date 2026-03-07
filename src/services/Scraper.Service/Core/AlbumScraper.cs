using System.Text.Json;

namespace Scraper.Service.Core;

public class AlbumScraper
{
    public (List<string> Ids, int Total) ParseAlbumsPage(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        int total = 0;
        if (root.TryGetProperty("total", out var totalProperty))
            total = totalProperty.GetInt32();

        var ids = new List<string>();
        if (root.TryGetProperty("items", out var items))
        {
            foreach (var item in items.EnumerateArray())
            {
                if (item.TryGetProperty("id", out var idProperty))
                    ids.Add(idProperty.GetString()!);
            }
        }

        return (ids, total);
    }
}