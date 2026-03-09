using System.Text.Json;
using Contracts;

namespace Scraper.Service.Core.Extractors;

public class AlbumExtractor
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
    
    public AlbumDetails ExtractFullAlbumInfo(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var id = root.GetProperty("id").GetString() ?? String.Empty;
        var name = root.GetProperty("name").GetString() ?? String.Empty;
        var releaseDate = root.GetProperty("release_date").GetString() ?? String.Empty;
        var totalTracks = root.GetProperty("total_tracks").GetInt32();
        
        string? imageUrl = null;
        if (root.TryGetProperty("images", out var images) && images.GetArrayLength() > 0)
        {
            imageUrl = images[0].GetProperty("url").GetString();
        }
        
        var tracks = new List<TrackInfo>();
        if (root.TryGetProperty("tracks", out var tracksRoot) && 
            tracksRoot.TryGetProperty("items", out var items))
        {
            foreach (var item in items.EnumerateArray())
            {
                tracks.Add(new TrackInfo(
                    item.GetProperty("name").GetString() ?? String.Empty,
                    item.GetProperty("duration_ms").GetInt32(),
                    item.GetProperty("track_number").GetInt32()
                ));
            }
        }

        return new AlbumDetails(id, name, releaseDate, totalTracks, imageUrl, tracks);
    }
}