namespace SuministrosDelEste.Application.DTOs;

/// <summary>
/// DTO para alertas de reabastecimiento del panel de inventario.
/// Contiene diagnóstico detallado del déficit de stock.
/// </summary>
public sealed record AlertaReabastecimientoDto
{
    public int MaterialId { get; init; }
    public string NombreMaterial { get; init; } = string.Empty;
    public decimal StockActual { get; init; }
    public decimal StockMinimo { get; init; }

    /// <summary>Unidades que faltan para alcanzar el stock mínimo.</summary>
    public decimal Deficit { get; init; }

    /// <summary>Mensaje de alerta generado por el IStockAlertStrategy configurado.</summary>
    public string Mensaje { get; init; } = string.Empty;

    /// <summary>True si el material está completamente sin stock.</summary>
    public bool EsAgotado { get; init; }
}
