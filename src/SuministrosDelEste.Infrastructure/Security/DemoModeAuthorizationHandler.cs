using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace SuministrosDelEste.Infrastructure.Security;

/// <summary>
/// Handler de autorización que permite todas las peticiones cuando DemoMode está activo.
/// Úsalo SOLO para demostración pública. En producción real, mantener DemoMode=false.
///
/// Activar en appsettings.Production.json → "Jwt": { "DemoMode": true }
/// Patrón: Strategy aplicado a autorización
/// </summary>
public sealed class DemoModeAuthorizationHandler : IAuthorizationHandler
{
    private readonly bool _isDemoMode;

    public DemoModeAuthorizationHandler(IConfiguration configuration)
    {
        _isDemoMode = configuration.GetValue<bool>("Jwt:DemoMode");
    }

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (_isDemoMode)
        {
            // En modo demo, aprobar automáticamente todos los requisitos de autorización
            foreach (IAuthorizationRequirement requirement in context.PendingRequirements.ToList())
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
