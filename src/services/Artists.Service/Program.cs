using Artists.Service.Core;
using Artists.Service;
using Artists.Service.Core.Dto;
using Artists.Service.Core.Repositories;
using Artists.Service.Core.Services;
using Artists.Service.Core.Validators;
using Dapper;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Shared;

[assembly: DapperAot]

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// builder.Services.AddMassTransit(x =>
// {
//     x.UsingRabbitMq((context, config) =>
//     {
//         config.Host(builder.Configuration.GetValue<string>("RabbitMq:Host"), "/", h => 
//         {
//             h.Username("guest");
//             h.Password("guest");
//         });
//     });
// });

// TODO move to utils?
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://keycloak:8080/auth/realms/music-catalog";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            NameClaimType = "preferred_username",
            RoleClaimType = "realm_access"
        };
    });

builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<IDbConnectionFactory>(_ => new PostgresDbConnectionFactory(connectionString));

builder.Services.AddScoped<IArtistsRepository, ArtistsRepository>();
builder.Services.AddScoped<IValidator<ArtistDto>, CreateArtistValidator>();
builder.Services.AddScoped<IArtistsService, ArtistsService>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseAuthentication();
app.UseAuthorization();

app.MapArtistEndpoints();

app.Run();