using MassTransit;
using Scraper.Service.Core.Consumers;
using Scraper.Service.Core.MusicServiceClient;
using Shared.Constants;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddLogging("scraper-service");

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
        
        config.UseMessageRetry(r => r.Incremental(
            BrokerConnection.RetryLimit,
            TimeSpan.FromSeconds(BrokerConnection.InitialInterval),
            TimeSpan.FromSeconds(BrokerConnection.IntervalIncrement))
        );
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