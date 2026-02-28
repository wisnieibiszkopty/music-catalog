using System.Text.Json.Serialization;

namespace Artist.Service;

[JsonSerializable(typeof(Artist))]
[JsonSerializable(typeof(List<Artist>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}