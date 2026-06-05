using SuministrosDelEste.Application.DTOs;
using SuministrosDelEste.Application.Ports;
using SuministrosDelEste.Application.Strategies;
using SuministrosDelEste.Domain.Aggregates.Material;
using SuministrosDelEste.Domain.Ports;

namespace SuministrosDelEste.Application.UseCases.ConsultarInventario;

/// <summary>
/// Handler del caso de uso: Consultar Inventario.
/// Usa el patrón Strategy para calcular y formatear alertas de reabastecimiento.
///
/// Patrón Strategy: el algoritmo de alerta es intercambiable via IStockAlertStrategy.
/// Principio SOLID OCP: se extiende con nuevas estrategias sin modificar este handler.
/// Principio SOLID DIP: depende de IMaterialRepository y IStockAlertStrategy (abstracciones).
/// </summary>
public sealed class ConsultarInventarioHandler : IConsultarInventarioUseCase
{
    private readonly IMaterialRepository _materialRepository;
    private readonly IStockAlertStrategy _alertStrategy;

    public ConsultarInventarioHandler(
        IMaterialRepository materialRepository,
        IStockAlertStrategy alertStrategy)
    {
        _materialRepository = materialRepository;
        _alertStrategy = alertStrategy;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MaterialDto>> ObtenerTodosAsync(ConsultarInventarioQuery query)
    {
        try
        {
            IEnumerable<Material> materiales = await _materialRepository.ObtenerTodosAsync();
            return materiales.Select(MaterialDto.DesdeEntidad).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"[ConsultarInventarioHandler.ObtenerTodosAsync] {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AlertaReabastecimientoDto>> ObtenerAlertasAsync(ConsultarInventarioQuery query)
    {
        try
        {
            IEnumerable<Material> materiales = await _materialRepository.ObtenerBajoStockMinimoAsync();

            return materiales.Select(m => new AlertaReabastecimientoDto
            {
                MaterialId  = m.MaterialId,
                NombreMaterial = m.Nombre.Valor,
                StockActual = m.StockActual.Valor,
                StockMinimo = m.StockMinimo.Valor,
                Deficit     = m.StockMinimo.Valor - m.StockActual.Valor,
                // Patrón Strategy: el mensaje lo genera la estrategia inyectada
                Mensaje     = _alertStrategy.ObtenerMensaje(m.Nombre.Valor, m.StockActual, m.StockMinimo),
                EsAgotado   = m.StockActual.EsAgotado()
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"[ConsultarInventarioHandler.ObtenerAlertasAsync] {ex.Message}", ex);
        }
    }
}
