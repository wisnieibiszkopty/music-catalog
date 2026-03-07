using System.Text.Json.Serialization;
using Contracts;
using MassTransit;
using Scraper.Service;
using Scraper.Service.Core;
using Scraper.Service.Core.Consumers;

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
    options.Configuration = builder.Configuration["Redis:Configuration"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

builder.Services.AddTransient<BearerTokenProvider>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DiscoverAlbumsConsumer>();
    x.AddConsumer<ScrapeAlbumDetailsConsumer>();
    x.AddConsumer<DiscoverArtistConsumer>();
    
    x.UsingRabbitMq((context, config) =>
    {
        config.Host("localhost");
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

app.MapScrapperEndpoints();

app.Run();

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(AlbumDetails))]
[JsonSerializable(typeof(ArtistDetails))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
