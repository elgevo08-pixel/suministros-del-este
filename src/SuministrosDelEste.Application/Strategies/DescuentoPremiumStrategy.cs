namespace SuministrosDelEste.Application.Strategies;

/// <summary>
/// Descuento fijo del 15% para clientes de categoría Premium.
/// Patrón: Strategy | Principio SOLID: OCP (ver IDescuentoStrategy).
/// </summary>
public sealed class DescuentoPremiumStrategy : IDescuentoStrategy
{
    private const decimal Porcentaje = 0.15m;

    /// <inheritdoc />
    public string TipoCliente => "Premium";

    /// <inheritdoc />
    public decimal Calcular(decimal montoCompra) => montoCompra <= 0 ? 0m : montoCompra * Porcentaje;
}
