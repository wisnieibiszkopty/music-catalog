using MassTransit;
using Scraper.Service;

var builder = WebApplication.CreateSlimBuilder(args);

// builder.Services.ConfigureHttpJsonOptions(options =>
// {
//     options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
// });

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer < ArtistSearchConsumer>();
    
    x.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration.GetValue<string>("RabbitMq:Host"), "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
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

app.Run();

// internal partial class AppJsonSerializerContext : JsonSerializerContext
// {
//
// }
