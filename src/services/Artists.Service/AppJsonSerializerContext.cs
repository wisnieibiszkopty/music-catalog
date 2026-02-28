using System.Text.Json.Serialization;
using Artists.Service.Core.Dto;
using Artists.Service.Core.Models;

namespace Artists.Service;

[JsonSerializable(typeof(Artist))]
[JsonSerializable(typeof(IEnumerable<Artist>))]
[JsonSerializable(typeof(List<Artist>))]
[JsonSerializable(typeof(ArtistBaseDto))]
[JsonSerializable(typeof(IEnumerable<ArtistBaseDto>))]
[JsonSerializable(typeof(List<ArtistBaseDto>))]
[JsonSerializable(typeof(ArtistDto))]
[JsonSerializable(typeof(IEnumerable<ArtistDto>))]
[JsonSerializable(typeof(List<ArtistDto>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}