using System.Text.Json.Serialization;
using MassTransit;
using Scraper.Service;
using Scraper.Service.Core;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddHttpClient<SpotifyClient>(client =>
{
    var baseAddress = builder.Configuration["Spotify:BaseAddress"]!;
    client.BaseAddress = new Uri(baseAddress);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

builder.Services.AddTransient<BearerTokenProvider>();

//
// builder.Services.AddMassTransit(x =>
// {
//     x.AddConsumer < ArtistSearchConsumer>();
//     
//     x.UsingRabbitMq((context, config) =>
//     {
//         config.Host(builder.Configuration.GetValue<string>("RabbitMq:Host"), "/", h =>
//         {
//             h.Username("guest");
//             h.Password("guest");
//         });
//         
//         config.ConfigureEndpoints(context);
//     });
// });

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
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
