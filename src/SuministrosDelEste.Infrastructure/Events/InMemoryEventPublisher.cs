using Microsoft.Extensions.Logging;
using SuministrosDelEste.Application.Ports;
using SuministrosDelEste.Domain.Events;

namespace SuministrosDelEste.Infrastructure.Events;

/// <summary>
/// Adaptador secundario para publicación de Domain Events.
/// Implementación en memoria orientada a desarrollo y pruebas.
///
/// Para producción, reemplazar este adaptador por uno de:
///   - MassTransit + RabbitMQ
///   - Azure Service Bus
///   - AWS SNS / SQS
/// El cambio requiere solo registrar el nuevo adaptador en DI — ningún código de negocio cambia.
///
/// Patrón Hexagonal: Secondary Adapter
/// Patrón de diseño: Observer (publicador / Subject)
/// </summary>
public sealed class InMemoryEventPublisher : IEventPublisher
{
    private readonly ILogger<InMemoryEventPublisher> _logger;

    public InMemoryEventPublisher(ILogger<InMemoryEventPublisher> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task PublicarAsync(IDomainEvent domainEvent)
    {
        _logger.LogInformation(
            "[DomainEvent] {EventType} | ID: {EventId} | Ocurrido: {OcurridoEn}",
            domainEvent.GetType().Name,
            domainEvent.EventId,
            domainEvent.OcurridoEn.ToString("yyyy-MM-dd HH:mm:ss") + " UTC");

        // TODO: Serializar y enviar al bus de mensajes en producción
        return Task.CompletedTask;
    }
}
