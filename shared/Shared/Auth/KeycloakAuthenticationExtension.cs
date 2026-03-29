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
                    ValidateIssuer = true,
                    ValidIssuer = "http://localhost:8080/auth/realms/music-catalog" 
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.Admin, policy => 
                policy.RequireAssertion(context => 
                    context.User.HasClaim(c => 
                        c.Type == "realm_access" && c.Value.Contains("admin"))));
        });

        return services;
    }
}