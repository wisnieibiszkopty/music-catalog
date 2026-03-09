using System.Text.Json;
using System.Text.Json.Serialization;
using Contracts;
using MassTransit;
using MassTransit.Events;
using MassTransit.Metadata;
using MassTransit.Serialization;
using Scraper.Service.Core;
using Scraper.Service.Core.Consumers;
using Scraper.Service.Core.MusicServiceClient;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddHttpClient<IMusicServiceClient, SpotifyClient>(client =>
{
    var baseAddress = builder.Configuration["MusicService:BaseAddress"]!;
    client.BaseAddress = new Uri(baseAddress);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ScrapperService";
});

builder.Services.AddSingleton<BearerTokenProvider>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DiscoverAlbumsConsumer>();
    x.AddConsumer<ScrapeAlbumDetailsConsumer>();
    x.AddConsumer<DiscoverArtistConsumer>();
    
    x.UsingRabbitMq((context, config) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("RabbitMq");
        config.Host(new Uri(connectionString!));
        config.ConfigureJsonSerializerOptions(options =>
        {
            options.TypeInfoResolver = AppJsonSerializerContext.Default;
            return options;
        });
        
        config.ConfigureEndpoints(context);
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var client = scope.ServiceProvider.GetService<IMusicServiceClient>();
    Console.WriteLine(client == null ? "NULL" : "OK");
}

app.Run();

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(AlbumDetails))]
[JsonSerializable(typeof(ArtistDetails))]
[JsonSerializable(typeof(DiscoverArtist))]
[JsonSerializable(typeof(FaultEvent<DiscoverArtist>))]
[JsonSerializable(typeof(SaveArtistData))]
[JsonSerializable(typeof(ArtistSaved))]
[JsonSerializable(typeof(ReceiveFaultEvent))]
[JsonSerializable(typeof(FaultExceptionInfo))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(MessageEnvelope))]
[JsonSerializable(typeof(JsonMessageEnvelope))]
[JsonSerializable(typeof(BusHostInfo))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
