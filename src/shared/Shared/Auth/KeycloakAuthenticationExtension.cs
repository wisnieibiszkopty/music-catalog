using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Shared.Auth;

public static class KeycloakAuthenticationExtension
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, string authority)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    NameClaimType = "preferred_username",
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = MapRoles();
            });

        services.AddAuthorization();

        return services;
    }

    private static JwtBearerEvents MapRoles()
    {
        return new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var principal = context.Principal;
                if (principal?.HasClaim(c => c.Type == "realm_access") == true)
                {
                    var realmAccessClaim = principal.FindFirst("realm_access")!.Value;
                    using var jsonDoc = JsonDocument.Parse(realmAccessClaim);
                    if (jsonDoc.RootElement.TryGetProperty("roles", out var roles))
                    {
                        var claims = roles.EnumerateArray()
                            .Select(r => new Claim(ClaimTypes.Role, r.GetString()!));
                        var identity = new ClaimsIdentity(claims);
                        principal.AddIdentity(identity);
                    }
                }
                
                return Task.CompletedTask;
            }
        };
    }
}