using SuministrosDelEste.Domain.Aggregates.Material.ValueObjects;

namespace SuministrosDelEste.Application.Strategies;

/// <summary>
/// Estrategia intercambiable para evaluar y describir alertas de reabastecimiento.
///
/// Patrón de diseño: Strategy — permite variar el algoritmo de alerta sin modificar el handler.
/// Principio SOLID OCP: nuevas estrategias se añaden sin tocar ConsultarInventarioHandler.
/// Principio SOLID ISP: interfaz mínima con cohesión de una sola responsabilidad.
/// </summary>
public interface IStockAlertStrategy
{
    /// <summary>Determina si los niveles de stock requieren una alerta.</summary>
    bool RequiereAlerta(Stock stockActual, Stock stockMinimo);

    /// <summary>Genera el mensaje descriptivo de la alerta para la UI o notificaciones.</summary>
    string ObtenerMensaje(string nombreMaterial, Stock stockActual, Stock stockMinimo);
}
