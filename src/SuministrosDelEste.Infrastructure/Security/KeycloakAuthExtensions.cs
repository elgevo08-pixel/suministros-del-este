using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SuministrosDelEste.Infrastructure.Security;

/// <summary>
/// Extensiones de IServiceCollection para configurar autenticación OAuth2/JWT con Keycloak.
///
/// Keycloak actúa como Identity Provider (IdP): emite tokens JWT firmados con RS256.
/// Este método configura la validación automática via OIDC Discovery Endpoint:
///   {Authority}/.well-known/openid-configuration
///
/// Patrón: Extension Method (Decorator sobre IServiceCollection)
/// </summary>
public static class KeycloakAuthExtensions
{
    /// <summary>
    /// Registra el middleware de autenticación JWT Bearer vinculado al realm de Keycloak.
    /// </summary>
    public static IServiceCollection AddKeycloakAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        JwtSettings settings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException(
                "Sección 'Jwt' no encontrada en appsettings.json. " +
                "Configure Authority (URL del realm) y Audience (client ID de Keycloak).");

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = settings.Authority;
                options.Audience = settings.Audience;
                options.RequireHttpsMetadata = settings.RequireHttpsMetadata;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer    = settings.Authority,
                    ValidateAudience = true,
                    ValidAudience    = settings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                    // Keycloak emite roles en el claim "realm_access.roles"
                    RoleClaimType = "realm_access.roles"
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.Error.WriteLine(
                            $"[Keycloak Auth] Fallo de autenticación: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
