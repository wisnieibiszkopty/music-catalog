using Artists.Service.Core;
using Artists.Service;
using Artists.Service.Core.Consumers;
using Artists.Service.Core.Dto;
using Artists.Service.Core.Repositories;
using Artists.Service.Core.Services;
using Artists.Service.Core.Validators;
using Dapper;
using FluentValidation;
using MassTransit;
using Scalar.AspNetCore;
using Shared;
using Shared.Auth;
using Shared.Errors;

[assembly: DapperAot]

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SaveArtistDataConsumer>();
    
    x.UsingRabbitMq((context, config) =>
    {
        var connectionString = builder.Configuration.GetConnectionString("RabbitMq")!;
        config.Host(new Uri(connectionString));
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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<IDbConnectionFactory>(_ => new PostgresDbConnectionFactory(connectionString));

builder.Services.AddScoped<IArtistsRepository, ArtistsRepository>();
builder.Services.AddScoped<IValidator<ArtistDto>, CreateArtistValidator>();
builder.Services.AddScoped<IArtistsService, ArtistsService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.MapArtistEndpoints();

app.Run();