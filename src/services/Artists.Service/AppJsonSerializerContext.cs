using System.Text.Json.Serialization;

namespace Artists.Service;

[JsonSerializable(typeof(Core.Models.Artist))]
[JsonSerializable(typeof(IEnumerable<Core.Models.Artist>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}