namespace SuministrosDelEste.Domain.Events;

/// <summary>
/// Domain Event: se eleva cuando el stock de un material cae al nivel mínimo o por debajo.
/// Activa flujos de alerta y reabastecimiento en el módulo de inventario.
/// Patrón: Domain Event (Observer)
/// </summary>
public sealed record StockBajoMinimoEvent(
    string NombreMaterial,
    decimal StockActual,
    decimal StockMinimo
) : IDomainEvent
{
    // init (no solo get): ver el comentario en MaterialRegistradoEvent — necesario para que el
    // patrón Outbox reconstruya el evento exacto desde su JSON persistido.
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OcurridoEn { get; init; } = DateTime.UtcNow;

    /// <summary>Cantidad de unidades que faltan para alcanzar el mínimo.</summary>
    public decimal Deficit { get; } = StockMinimo - StockActual;
}
