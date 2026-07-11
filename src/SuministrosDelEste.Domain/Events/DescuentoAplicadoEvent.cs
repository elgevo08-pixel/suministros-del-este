namespace SuministrosDelEste.Domain.Events;

/// <summary>
/// Domain Event: se eleva cada vez que se calcula un descuento para una compra.
/// Reutiliza el mismo puerto Observer (IEventPublisher) que ya usa el módulo de
/// Inventario — no se inventó un mecanismo de notificación nuevo para este caso de uso.
/// Patrón: Domain Event (Observer)
/// </summary>
public sealed record DescuentoAplicadoEvent(
    string TipoCliente,
    decimal MontoCompra,
    decimal MontoDescuento
) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OcurridoEn { get; init; } = DateTime.UtcNow;
}
