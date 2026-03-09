using System.Text.Json.Serialization;
using Artists.Service.Core.Dto;
using Artists.Service.Core.Models;
using Contracts;
using MassTransit.Metadata;
using MassTransit.Serialization;

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
[JsonSerializable(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
[JsonSerializable(typeof(DiscoverArtist))]
[JsonSerializable(typeof(SaveArtistData))]
[JsonSerializable(typeof(ArtistSaved))]
[JsonSerializable(typeof(MessageEnvelope))]
[JsonSerializable(typeof(JsonMessageEnvelope))]
[JsonSerializable(typeof(BusHostInfo))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}