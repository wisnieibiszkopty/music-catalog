using Catalog.Service.Core;
using Catalog.Service.Core.Consumers;
using Catalog.Service.Core.Models;
using Catalog.Service.Core.Services;
using Catalog.Service.Core.Validators;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Auth;
using Shared.Errors;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddLogging("catalog-service");

builder.Services.AddDbContext<CatalogDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention()
);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SaveAlbumDataConsumer>();
    x.AddConsumer<ArtistDeletedConsumer>();
    
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

builder.Services.AddScoped<IValidator<Album>, AlbumValidator>();
builder.Services.AddScoped<ICatalogService, CatalogService>();

builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapCatalogEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

app.Run();