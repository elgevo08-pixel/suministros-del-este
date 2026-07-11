namespace SuministrosDelEste.Application.Strategies;

/// <summary>
/// Descuento fijo del 10% para clientes frecuentes.
/// Patrón: Strategy | Principio SOLID: OCP (ver IDescuentoStrategy).
/// </summary>
public sealed class DescuentoFrecuenteStrategy : IDescuentoStrategy
{
    private const decimal Porcentaje = 0.10m;

    /// <inheritdoc />
    public string TipoCliente => "Frecuente";

    /// <inheritdoc />
    public decimal Calcular(decimal montoCompra) => montoCompra <= 0 ? 0m : montoCompra * Porcentaje;
}
