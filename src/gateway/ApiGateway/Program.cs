using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // TODO get paths from config
        options.MetadataAddress = "http://keycloak:8080/auth/realms/music-catalog/.well-known/openid-configuration"; 
        options.RequireHttpsMetadata = false; 
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = "api-client",
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:8080/auth/realms/music-catalog", 
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

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
