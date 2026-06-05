namespace SuministrosDelEste.Domain.Events;

/// <summary>
/// Interfaz marcadora para todos los Domain Events del sistema.
/// Patrón DDD: Domain Event | Patrón de diseño: Observer (contrato del evento)
/// </summary>
public interface IDomainEvent
{
    /// <summary>Identificador único del evento para trazabilidad.</summary>
    Guid EventId { get; }

    /// <summary>Fecha y hora UTC en que ocurrió el evento.</summary>
    DateTime OcurridoEn { get; }
}
