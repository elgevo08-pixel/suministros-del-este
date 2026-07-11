using System.Text.Json;
using SuministrosDelEste.Domain.Events;

namespace SuministrosDelEste.Infrastructure.Persistence.Outbox;

/// <summary>
/// Registro de persistencia del patrón Outbox: guarda cada <see cref="IDomainEvent"/>
/// en la misma transacción y el mismo SaveChangesAsync que el Aggregate que lo generó
/// (ver <see cref="Context.AppDbContext.SaveChangesAsync"/>), garantizando que un evento
/// nunca se pierda aunque el proceso falle justo después de persistir el Aggregate.
///
/// Un proceso en segundo plano (<see cref="BackgroundServices.OutboxDispatcherService"/>)
/// despacha estos mensajes de forma asíncrona hacia <see cref="Application.Ports.IEventPublisher"/>.
///
/// Patrón: Outbox (Transactional Outbox) — complementa al patrón Observer/Domain Events
/// ya existente en el Aggregate Root, sin modificarlo.
/// No es un concepto de Dominio: es un detalle técnico de persistencia, por eso vive en
/// Infrastructure y no en Domain.
/// </summary>
public sealed class OutboxMessage
{
    /// <summary>Identificador del mensaje. Coincide con el EventId del Domain Event original.</summary>
    public Guid Id { get; private set; }

    /// <summary>Nombre completo (namespace + clase) del tipo de Domain Event serializado.</summary>
    public string TipoEvento { get; private set; } = string.Empty;

    /// <summary>Payload del evento serializado en JSON, tal como fue elevado por el Aggregate.</summary>
    public string ContenidoJson { get; private set; } = string.Empty;

    /// <summary>Fecha y hora UTC en que ocurrió el evento original (no cuándo se despachó).</summary>
    public DateTime OcurridoEn { get; private set; }

    /// <summary>Fecha y hora UTC en que el mensaje fue despachado exitosamente. Null mientras esté pendiente.</summary>
    public DateTime? ProcesadoEn { get; private set; }

    /// <summary>Cantidad de intentos de despacho realizados.</summary>
    public int Intentos { get; private set; }

    /// <summary>Mensaje de la última excepción al intentar despachar, si la hubo.</summary>
    public string? UltimoError { get; private set; }

    /// <summary>Constructor privado requerido por EF Core para materialización desde BD.</summary>
    private OutboxMessage() { }

    /// <summary>
    /// Única vía válida para crear un OutboxMessage a partir de un Domain Event recién elevado.
    /// Patrón: Factory Method (consistente con Material.Crear / los Value Objects .Crear).
    /// </summary>
    public static OutboxMessage DesdeEvento(IDomainEvent domainEvent)
    {
        Type tipo = domainEvent.GetType();

        return new OutboxMessage
        {
            Id = domainEvent.EventId,
            TipoEvento = tipo.FullName ?? tipo.Name,
            ContenidoJson = JsonSerializer.Serialize(domainEvent, tipo),
            OcurridoEn = domainEvent.OcurridoEn,
            Intentos = 0
        };
    }

    /// <summary>
    /// Reconstruye el Domain Event original a partir del payload guardado.
    /// Retorna null si el tipo ya no existe en el ensamblado de Domain (evento obsoleto/renombrado).
    /// </summary>
    public IDomainEvent? Deserializar()
    {
        Type? tipo = typeof(IDomainEvent).Assembly.GetType(TipoEvento);
        if (tipo is null)
            return null;

        return (IDomainEvent?)JsonSerializer.Deserialize(ContenidoJson, tipo);
    }

    /// <summary>Marca el mensaje como despachado exitosamente.</summary>
    public void MarcarProcesado() => ProcesadoEn = DateTime.UtcNow;

    /// <summary>Registra un intento fallido de despacho, incrementando el contador de reintentos.</summary>
    public void RegistrarError(string mensaje)
    {
        Intentos++;
        UltimoError = mensaje.Length > 500 ? mensaje[..500] : mensaje;
    }
}
