using SuministrosDelEste.Application.DTOs;
using SuministrosDelEste.Application.Factories;
using SuministrosDelEste.Application.Ports;
using SuministrosDelEste.Domain.Aggregates.Material;
using SuministrosDelEste.Domain.Exceptions;
using SuministrosDelEste.Domain.Ports;

namespace SuministrosDelEste.Application.UseCases.RegistrarMaterial;

/// <summary>
/// Handler del caso de uso: Registrar Material en el inventario.
/// Orquesta: Factory → Repositorio (persiste el Aggregate y su Outbox transaccional) → DTO de respuesta.
///
/// Patrones aplicados:
///   - Factory: construye el Aggregate validado via IMaterialFactory
///   - Repository: persiste via IMaterialRepository (puerto del dominio)
///   - Outbox: IMaterialRepository.GuardarAsync ya deja los Domain Events guardados de forma
///     durable en la misma transacción (ver AppDbContext.SaveChangesAsync). Este handler ya NO
///     publica los eventos directamente contra IEventPublisher — eso ahora lo hace de forma
///     asíncrona OutboxDispatcherService, para no bloquear la petición HTTP ni arriesgar
///     perder el evento si el proceso falla justo después de guardar.
///
/// Principios SOLID:
///   - SRP: única responsabilidad, orquestar el flujo de registro
///   - DIP: depende de abstracciones (puertos), nunca de implementaciones concretas
/// </summary>
public sealed class RegistrarMaterialHandler : IRegistrarMaterialUseCase
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IMaterialFactory _materialFactory;

    public RegistrarMaterialHandler(
        IMaterialRepository materialRepository,
        IMaterialFactory materialFactory)
    {
        _materialRepository = materialRepository;
        _materialFactory = materialFactory;
    }

    /// <inheritdoc />
    public async Task<MaterialDto> EjecutarAsync(RegistrarMaterialCommand command)
    {
        try
        {
            // 1. Construir el Aggregate Root via Factory Pattern
            Material material = _materialFactory.Crear(command);

            // 2. Persistir el agregado via Outbound Port (Hexagonal).
            //    GuardarAsync dispara AppDbContext.SaveChangesAsync, que captura los Domain
            //    Events del Aggregate hacia el Outbox en la MISMA transacción (patrón Outbox).
            //    El despacho real hacia IEventPublisher ocurre después, en segundo plano,
            //    vía OutboxDispatcherService — no aquí.
            await _materialRepository.GuardarAsync(material);

            // 3. Retornar DTO de respuesta al puerto de entrada
            return MaterialDto.DesdeEntidad(material);
        }
        catch (DomainException)
        {
            throw; // Las excepciones de dominio se propagan sin envolver (→ HTTP 400)
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"[RegistrarMaterialHandler] Error inesperado al registrar '{command.Nombre}': {ex.Message}", ex);
        }
    }
}
