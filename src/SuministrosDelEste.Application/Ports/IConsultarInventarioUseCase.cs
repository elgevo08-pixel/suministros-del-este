using SuministrosDelEste.Application.DTOs;
using SuministrosDelEste.Application.UseCases.ConsultarInventario;

namespace SuministrosDelEste.Application.Ports;

/// <summary>
/// Puerto de entrada (Inbound / Primary Port) para consultas del inventario.
/// Patrón Hexagonal: Primary Port | Principio SOLID ISP.
/// </summary>
public interface IConsultarInventarioUseCase
{
    /// <summary>Retorna el catálogo completo de materiales activos.</summary>
    Task<IEnumerable<MaterialDto>> ObtenerTodosAsync(ConsultarInventarioQuery query);

    /// <summary>Retorna los materiales con stock en nivel crítico o agotado.</summary>
    Task<IEnumerable<AlertaReabastecimientoDto>> ObtenerAlertasAsync(ConsultarInventarioQuery query);
}
