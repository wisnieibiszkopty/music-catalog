using System.Text.Json.Serialization;
using Contracts;
using MassTransit;
using Orchestrator.Service;
using Orchestrator.Service.Core.Saga;
using Shared.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<AlbumScraperSaga, AlbumScraperState>()
        .RedisRepository(redis =>
        {
            var redisConnection = builder.Configuration.GetConnectionString("Redis");
            redis.DatabaseConfiguration(redisConnection);
            redis.KeyPrefix = "orchestrator";
        });
    
    x.UsingRabbitMq((context, config) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("RabbitMq")!;
        config.Host(new Uri(connectionString));
        config.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
        
        config.ConfigureJsonSerializerOptions(options =>
        {
            options.TypeInfoResolver = AppJsonSerializerContext.Default;
            return options;
        });
        
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddKeycloakAuthentication("http://keycloak:8080/auth/realms/music-catalog");

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapScrappingEndpoints();

app.Run();

[JsonSerializable(typeof(StartAlbumsScraping))]
[JsonSerializable(typeof(DiscoverAlbums))]
[JsonSerializable(typeof(AlbumsDiscovered))]
[JsonSerializable(typeof(ScrapeAlbumDetails))]
[JsonSerializable(typeof(SaveAlbumData))]
[JsonSerializable(typeof(AlbumSaved))]
[JsonSerializable(typeof(AllAlbumsScraped))]
[JsonSerializable(typeof(AlbumScraperState))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }