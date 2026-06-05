using SuministrosDelEste.Application.DTOs;
using SuministrosDelEste.Application.Factories;
using SuministrosDelEste.Application.Ports;
using SuministrosDelEste.Domain.Aggregates.Material;
using SuministrosDelEste.Domain.Events;
using SuministrosDelEste.Domain.Exceptions;
using SuministrosDelEste.Domain.Ports;

namespace SuministrosDelEste.Application.UseCases.RegistrarMaterial;

/// <summary>
/// Handler del caso de uso: Registrar Material en el inventario.
/// Orquesta: Factory → Repositorio → Publicación de Eventos → DTO de respuesta.
///
/// Patrones aplicados:
///   - Factory: construye el Aggregate validado via IMaterialFactory
///   - Repository: persiste via IMaterialRepository (puerto del dominio)
///   - Observer: publica Domain Events via IEventPublisher (puerto de aplicación)
///
/// Principios SOLID:
///   - SRP: única responsabilidad, orquestar el flujo de registro
///   - DIP: depende de abstracciones (puertos), nunca de implementaciones concretas
/// </summary>
public sealed class RegistrarMaterialHandler : IRegistrarMaterialUseCase
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IMaterialFactory _materialFactory;
    private readonly IEventPublisher _eventPublisher;

    public RegistrarMaterialHandler(
        IMaterialRepository materialRepository,
        IMaterialFactory materialFactory,
        IEventPublisher eventPublisher)
    {
        _materialRepository = materialRepository;
        _materialFactory = materialFactory;
        _eventPublisher = eventPublisher;
    }

    /// <inheritdoc />
    public async Task<MaterialDto> EjecutarAsync(RegistrarMaterialCommand command)
    {
        try
        {
            // 1. Construir el Aggregate Root via Factory Pattern
            Material material = _materialFactory.Crear(command);

            // 2. Persistir el agregado via Outbound Port (Hexagonal)
            await _materialRepository.GuardarAsync(material);

            // 3. Publicar Domain Events generados por el agregado — patrón Observer
            foreach (IDomainEvent domainEvent in material.DomainEvents)
                await _eventPublisher.PublicarAsync(domainEvent);

            material.ClearDomainEvents();

            // 4. Retornar DTO de respuesta al puerto de entrada
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
