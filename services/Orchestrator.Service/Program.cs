using MassTransit;
using Microsoft.AspNetCore.DataProtection;
using Orchestrator.Service.Core;
using Orchestrator.Service.Core.Saga;
using Shared.Auth;
using Shared.Constants;
using Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddObservability(builder.Environment.ApplicationName);

builder.Services.AddDataProtection()
    .UseEphemeralDataProtectionProvider();

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
        
        config.UseMessageRetry(r => r.Incremental(
            BrokerConnection.RetryLimit,
            TimeSpan.FromSeconds(BrokerConnection.InitialInterval),
            TimeSpan.FromSeconds(BrokerConnection.IntervalIncrement))
        );
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddKeycloakAuthentication(
    $"{builder.Configuration["Keycloak:InternalUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}"
);

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