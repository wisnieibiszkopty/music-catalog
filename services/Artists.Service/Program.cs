using Artists.Service.Core;
using Artists.Service.Core.Consumers;
using Artists.Service.Core.Dto;
using Artists.Service.Core.Repositories;
using Artists.Service.Core.Services;
using Artists.Service.Core.Validators;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.DataProtection;
using Scalar.AspNetCore;
using Shared;
using Shared.Auth;
using Shared.Constants;
using Shared.Errors;
using Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection()
    .UseEphemeralDataProtectionProvider();

builder.Host.AddObservability(builder.Environment.ApplicationName);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SaveArtistDataConsumer>();
    
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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<IDbConnectionFactory>(_ => new PostgresDbConnectionFactory(connectionString));

builder.Services.AddScoped<IArtistsRepository, ArtistsRepository>();
builder.Services.AddScoped<IValidator<ArtistDto>, CreateArtistValidator>();
builder.Services.AddScoped<IArtistsService, ArtistsService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapArtistEndpoints();

app.Run();