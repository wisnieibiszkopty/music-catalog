using System.Text.Json.Serialization;

namespace Artist.Service;

[JsonSerializable(typeof(Core.Artist))]
[JsonSerializable(typeof(IEnumerable<Core.Artist>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}