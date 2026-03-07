using System.Text.Json.Serialization;
using Contracts;
using MassTransit;
using Orchestrator.Service;
using Orchestrator.Service.Core.Saga;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<AlbumScraperSaga, AlbumScraperState>()
        .InMemoryRepository();
    
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

app.MapScrappingEndpoints();

app.Run();



[JsonSerializable(typeof(StartAlbumsScraping))]
[JsonSerializable(typeof(DiscoverAlbums))]
[JsonSerializable(typeof(AlbumsDiscovered))]
[JsonSerializable(typeof(ScrapeAlbumDetails))]
[JsonSerializable(typeof(SaveAlbumData))]
[JsonSerializable(typeof(AlbumSaved))]
[JsonSerializable(typeof(AllAlbumsScraped))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }