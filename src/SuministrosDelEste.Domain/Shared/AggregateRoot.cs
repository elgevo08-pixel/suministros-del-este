using SuministrosDelEste.Domain.Events;

namespace SuministrosDelEste.Domain.Shared;

/// <summary>
/// Clase base para todos los Aggregate Roots del dominio.
/// Gestiona la colección de Domain Events pendientes de publicación.
/// Patrón DDD: Aggregate Root | Patrón de diseño: Observer (productor)
/// </summary>
public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Eventos de dominio generados durante la sesión actual del agregado.
    /// Son publicados por la capa de Aplicación tras cada operación exitosa.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Añade un Domain Event a la cola interna del agregado.
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Limpia los eventos tras su publicación exitosa.
    /// </summary>
    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
