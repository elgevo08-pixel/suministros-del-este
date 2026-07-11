using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SuministrosDelEste.Application.Ports;
using SuministrosDelEste.Domain.Events;
using SuministrosDelEste.Infrastructure.Persistence.Context;
using SuministrosDelEste.Infrastructure.Persistence.Outbox;

namespace SuministrosDelEste.Infrastructure.BackgroundServices;

/// <summary>
/// Adaptador secundario que despacha en segundo plano los <see cref="OutboxMessage"/> pendientes
/// hacia <see cref="IEventPublisher"/>, desacoplando la publicación del ciclo de vida de la
/// petición HTTP (resuelve el procesamiento síncrono señalado en el diagnóstico de la Unidad II).
///
/// Al ejecutarse fuera de una petición, crea su propio scope de DI en cada ciclo (AppDbContext
/// e IEventPublisher son Scoped, no se pueden inyectar directo en un servicio Singleton).
///
/// Patrón Hexagonal: Secondary Adapter | Patrón: Outbox (consumidor) + Observer (via IEventPublisher)
/// </summary>
public sealed class OutboxDispatcherService : BackgroundService
{
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(5);
    private const int MaxIntentos = 5;
    private const int TamanoLote = 20;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxDispatcherService> _logger;

    public OutboxDispatcherService(IServiceScopeFactory scopeFactory, ILogger<OutboxDispatcherService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[OutboxDispatcherService] Iniciado. Intervalo de despacho: {Intervalo}s.", Intervalo.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DespacharPendientesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                // No dejar morir el BackgroundService por un fallo puntual (ej. BD caída momentáneamente).
                _logger.LogError(ex, "[OutboxDispatcherService] Error inesperado despachando eventos pendientes.");
            }

            try
            {
                await Task.Delay(Intervalo, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Apagado normal del host — no es un error.
            }
        }
    }

    /// <summary>
    /// Toma un lote de mensajes pendientes (no procesados y con intentos por debajo del máximo)
    /// y los publica uno a uno, registrando éxito o error individualmente.
    /// </summary>
    private async Task DespacharPendientesAsync(CancellationToken ct)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        IEventPublisher publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

        List<OutboxMessage> pendientes = await context.OutboxMessages
            .Where(m => m.ProcesadoEn == null && m.Intentos < MaxIntentos)
            .OrderBy(m => m.OcurridoEn)
            .Take(TamanoLote)
            .ToListAsync(ct);

        if (pendientes.Count == 0)
            return;

        foreach (OutboxMessage mensaje in pendientes)
        {
            try
            {
                IDomainEvent? evento = mensaje.Deserializar();
                if (evento is null)
                {
                    mensaje.RegistrarError($"Tipo de evento no reconocido: {mensaje.TipoEvento}");
                    continue;
                }

                await publisher.PublicarAsync(evento);
                mensaje.MarcarProcesado();
            }
            catch (Exception ex)
            {
                mensaje.RegistrarError(ex.Message);
                _logger.LogWarning(ex, "[OutboxDispatcherService] Falló el despacho del mensaje {MensajeId}.", mensaje.Id);
            }
        }

        await context.SaveChangesAsync(ct);
        _logger.LogInformation("[OutboxDispatcherService] Lote procesado: {Cantidad} mensaje(s).", pendientes.Count);
    }
}
