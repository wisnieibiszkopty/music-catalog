using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateSlimBuilder(args);

// TODO replace
var secretKey =
    "LongKeyLongKeyLongKeyLongKeyLongKeyLongKeyLongKeyLongKeyLongKeyLongKeyLongKeyLongKeyLongKeyLongKeyLongKey";

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "MusicCatalogIdentity",
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthPolicy", policy => policy.RequireAuthenticatedUser());
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
