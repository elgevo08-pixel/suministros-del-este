using SuministrosDelEste.Domain.Aggregates.Material.ValueObjects;

namespace SuministrosDelEste.Application.Strategies;

/// <summary>
/// Estrategia concreta: alerta cuando StockActual ≤ StockMinimo.
/// Implementación por defecto del módulo de inventario de Suministros del Este.
///
/// Para cambiar el criterio de alerta (ej: 20% sobre el mínimo) basta con registrar
/// una nueva estrategia en el DI — sin modificar ningún handler (OCP).
///
/// Patrón de diseño: Strategy (implementación concreta)
/// </summary>
public sealed class StockMinimoAlertStrategy : IStockAlertStrategy
{
    /// <inheritdoc />
    public bool RequiereAlerta(Stock stockActual, Stock stockMinimo)
        => stockActual.EsMenorOIgualQue(stockMinimo);

    /// <inheritdoc />
    public string ObtenerMensaje(string nombreMaterial, Stock stockActual, Stock stockMinimo)
    {
        string nivel = stockActual.EsAgotado() ? "⛔ AGOTADO" : "⚠️  CRÍTICO";
        decimal deficit = stockMinimo.Valor - stockActual.Valor;

        return $"{nivel} | {nombreMaterial}: stock actual {stockActual.Valor:F2}, " +
               $"mínimo requerido {stockMinimo.Valor:F2}. Déficit: {deficit:F2} unidades.";
    }
}
