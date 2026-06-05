using SuministrosDelEste.Domain.Events;

namespace SuministrosDelEste.Application.Ports;

/// <summary>
/// Puerto de salida (Outbound Port) para la publicación de Domain Events.
/// La Aplicación define qué necesita; la Infraestructura provee el adaptador concreto.
///
/// Patrón Hexagonal: Secondary Port
/// Patrón de diseño: Observer (publicador del evento)
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publica un Domain Event hacia el bus de eventos configurado.
    /// </summary>
    Task PublicarAsync(IDomainEvent domainEvent);
}
