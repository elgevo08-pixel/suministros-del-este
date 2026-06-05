namespace SuministrosDelEste.Infrastructure.Security;

/// <summary>
/// POCO de configuración para autenticación JWT / OAuth2 / Keycloak.
/// Se carga automáticamente desde la sección "Jwt" de appsettings.json.
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    /// <summary>
    /// URL del realm de Keycloak.
    /// Ejemplo: http://keycloak:8080/realms/suministros
    /// </summary>
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del cliente (audience) registrado en Keycloak.
    /// Ejemplo: suministros-api
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// False en desarrollo (HTTP local), True en producción (HTTPS obligatorio).
    /// </summary>
    public bool RequireHttpsMetadata { get; set; } = true;
}
