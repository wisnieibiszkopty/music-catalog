using MassTransit;
using Notification.Service.Core.Consumers;
using Notification.Service.Core.Hubs;
using Shared.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ArtistSavedConsumer>();
    x.AddConsumer<AllAlbumsScrapedConsumer>();
    x.AddConsumer<ScrapingFailedConsumer>();
    
    x.UsingRabbitMq((context, config) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("RabbitMq");
        config.Host(new Uri(connectionString!));
        
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddKeycloakAuthentication(
    $"{builder.Configuration["Keycloak:InternalUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}"
);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<NotificationHub>("/api/notifications");

app.Run();
