using SuministrosDelEste.Application.DTOs;
using SuministrosDelEste.Application.Ports;
using SuministrosDelEste.Application.Strategies;
using SuministrosDelEste.Domain.Events;
using SuministrosDelEste.Domain.Exceptions;

namespace SuministrosDelEste.Application.UseCases.CalcularDescuento;

/// <summary>
/// Handler del caso de uso: Calcular Descuento.
/// Orquesta: Strategy (según tipo de cliente) → Domain Event → DTO de respuesta.
///
/// Contraste con docs/solid-antes-despues/antes/CalculadoraDescuentoService.cs:
///   - SRP: esta clase SOLO orquesta el cálculo. No audita ni notifica por su cuenta;
///     delega la notificación al puerto ya existente IEventPublisher.
///   - OCP: no tiene ningún if/else por tipo de cliente — resuelve la estrategia
///     correcta desde el diccionario inyectado. Un tipo de cliente nuevo no toca
///     este archivo.
///   - DIP: depende de IDescuentoStrategy e IEventPublisher (abstracciones,
///     inyectadas por constructor), nunca de una clase concreta con "new".
///
/// Principios SOLID: SRP (única razón para cambiar: cómo se orquesta el cálculo)
///                    OCP (abierto a nuevas estrategias, cerrado a modificación)
///                    DIP (depende de abstracciones inyectadas)
/// </summary>
public sealed class CalcularDescuentoHandler : ICalcularDescuentoUseCase
{
    private readonly IReadOnlyDictionary<string, IDescuentoStrategy> _estrategias;
    private readonly IEventPublisher _eventPublisher;

    public CalcularDescuentoHandler(IEnumerable<IDescuentoStrategy> estrategias, IEventPublisher eventPublisher)
    {
        _estrategias = estrategias.ToDictionary(e => e.TipoCliente, e => e, StringComparer.OrdinalIgnoreCase);
        _eventPublisher = eventPublisher;
    }

    /// <inheritdoc />
    public async Task<DescuentoCalculadoDto> EjecutarAsync(CalcularDescuentoCommand command)
    {
        try
        {
            if (command.MontoCompra < 0)
                throw new DomainException("El monto de compra no puede ser negativo.");

            // Sin if/else por tipo — cualquier estrategia registrada en el DI aparece aquí.
            decimal descuento = _estrategias.TryGetValue(command.TipoCliente, out IDescuentoStrategy? estrategia)
                ? estrategia.Calcular(command.MontoCompra)
                : 0m; // tipo de cliente no reconocido = sin descuento, nunca una excepción (contrato LSP)

            await _eventPublisher.PublicarAsync(
                new DescuentoAplicadoEvent(command.TipoCliente, command.MontoCompra, descuento));

            return new DescuentoCalculadoDto
            {
                TipoCliente = command.TipoCliente,
                MontoCompra = command.MontoCompra,
                MontoDescuento = descuento,
                MontoFinal = command.MontoCompra - descuento
            };
        }
        catch (DomainException)
        {
            throw; // Regla de negocio: el adaptador REST la traduce a 400
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"[CalcularDescuentoHandler] Error calculando descuento para '{command.TipoCliente}': {ex.Message}", ex);
        }
    }
}
