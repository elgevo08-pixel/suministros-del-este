using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuministrosDelEste.Application.Ports;
using SuministrosDelEste.Domain.Ports;
using SuministrosDelEste.Infrastructure.BackgroundServices;
using SuministrosDelEste.Infrastructure.Events;
using SuministrosDelEste.Infrastructure.Persistence.Context;
using SuministrosDelEste.Infrastructure.Persistence.Repositories;
using SuministrosDelEste.Infrastructure.Security;

namespace SuministrosDelEste.Infrastructure.DependencyInjection;

/// <summary>
/// Extensión de IServiceCollection: registra todos los servicios de la capa de Infraestructura.
/// Soporta SQL Server (producción) y SQLite (demo/Railway.app) detectado automáticamente.
///
/// Principio SOLID SRP: centraliza el registro de adaptadores secundarios.
/// Patrón: Extension Method / Composition Root
/// </summary>
public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connection = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("'DefaultConnection' no configurado en appsettings.");

        // Detectar proveedor: SQLite para demo (Data Source=...) o SQL Server para producción
        bool useSqlite = connection.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase);

        services.AddDbContext<AppDbContext>(options =>
        {
            if (useSqlite)
            {
                options.UseSqlite(connection);
            }
            else
            {
                options.UseSqlServer(connection,
                    sql => sql.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null));
            }
        });

        // Repository — implementa el puerto de dominio IMaterialRepository
        services.AddScoped<IMaterialRepository, MaterialRepository>();

        // Event Publisher — implementa el puerto de aplicación IEventPublisher
        services.AddScoped<IEventPublisher, InMemoryEventPublisher>();

        // Patrón Outbox: despacha en segundo plano los OutboxMessage pendientes hacia IEventPublisher.
        // La durabilidad ya la garantiza AppDbContext.SaveChangesAsync (ver Persistence/Context);
        // este servicio solo se encarga de la entrega asíncrona, fuera del ciclo de la petición HTTP.
        services.AddHostedService<OutboxDispatcherService>();

        // Autenticación JWT / OAuth2 / Keycloak
        services.AddKeycloakAuthentication(configuration);

        // DemoMode: si Jwt:DemoMode=true, auto-aprueba todas las peticiones (solo para demo público)
        bool isDemoMode = configuration.GetValue<bool>("Jwt:DemoMode");
        if (isDemoMode)
            services.AddSingleton<IAuthorizationHandler, DemoModeAuthorizationHandler>();

        return services;
    }
}
