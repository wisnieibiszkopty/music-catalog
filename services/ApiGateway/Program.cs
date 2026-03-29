using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MetadataAddress = $"{builder.Configuration["Keycloak:InternalUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}/.well-known/openid-configuration"; 
        options.RequireHttpsMetadata = false; 
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Keycloak:Audience"],
            ValidateIssuer = true,
            ValidIssuer = $"{builder.Configuration["Keycloak:ExternalUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}", 
            NameClaimType = "preferred_username"
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthPolicy", policy => policy.RequireAuthenticatedUser());
});

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
