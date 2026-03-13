using MassTransit;
using Notification.Service.Core.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ArtistSavedConsumer>();
    x.AddConsumer<AllAlbumsScrapedConsumer>();
    
    x.UsingRabbitMq((context, config) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("RabbitMq");
        config.Host(new Uri(connectionString!));
        
        config.ConfigureEndpoints(context);
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
