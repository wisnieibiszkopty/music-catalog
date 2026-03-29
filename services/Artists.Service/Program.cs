using Artists.Service.Core;
using Artists.Service.Core.Consumers;
using Artists.Service.Core.Dto;
using Artists.Service.Core.Repositories;
using Artists.Service.Core.Services;
using Artists.Service.Core.Validators;
using FluentValidation;
using MassTransit;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Shared;
using Shared.Auth;
using Shared.Errors;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddLogging("artists-service");

// TODO move to extenstion method
var otel = builder.Services.AddOpenTelemetry();

otel.ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName));
otel.WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddRuntimeInstrumentation()
    .AddPrometheusExporter());

otel.WithTracing(tracing =>
{
    tracing.AddAspNetCoreInstrumentation(options =>
        {
            options.Filter = httpContext => 
                !httpContext.Request.Path.Value!.Contains("/metrics") && 
                !httpContext.Request.Path.Value!.Contains("/health");
            options.RecordException = true;
        })
        .AddHttpClientInstrumentation()
        .AddSource("MassTransit");
    
    var endpoint = builder.Configuration["Oltp:Endpoint"];
    if (!string.IsNullOrEmpty(endpoint))
    {
        tracing.AddOtlpExporter(opt => opt.Endpoint = new Uri(endpoint));
    }
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SaveArtistDataConsumer>();
    
    x.UsingRabbitMq((context, config) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("RabbitMq")!;
        config.Host(new Uri(connectionString));
        
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