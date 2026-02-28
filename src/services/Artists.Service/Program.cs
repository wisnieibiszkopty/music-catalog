using Artists.Service.Core;
using Artists.Service;
using Dapper;
using Scalar.AspNetCore;

[assembly: DapperAot]

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton<IDbConnectionFactory>(_ => new PostgresDbConnectionFactory(connectionString));

builder.Services.AddScoped<IArtistService, ArtistService>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapArtistEndpoints();

app.Run();