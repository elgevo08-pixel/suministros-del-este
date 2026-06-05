using SuministrosDelEste.Application.DTOs;
using SuministrosDelEste.Application.UseCases.RegistrarMaterial;

namespace SuministrosDelEste.Application.Ports;

/// <summary>
/// Puerto de entrada (Inbound / Primary Port) para el registro de materiales.
/// La capa de Presentación (API) interactúa con este puerto, nunca con la implementación.
///
/// Patrón Hexagonal: Primary Port
/// Principio SOLID ISP: interfaz mínima y específica de un solo caso de uso.
/// </summary>
public interface IRegistrarMaterialUseCase
{
    Task<MaterialDto> EjecutarAsync(RegistrarMaterialCommand command);
}
