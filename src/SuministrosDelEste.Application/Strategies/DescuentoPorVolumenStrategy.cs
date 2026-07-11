namespace SuministrosDelEste.Application.Strategies;

/// <summary>
/// Descuento escalonado según el monto de la compra: 12% para compras que superan
/// el umbral de volumen, 5% para el resto.
/// Patrón: Strategy | Principio SOLID: OCP (ver IDescuentoStrategy).
/// </summary>
public sealed class DescuentoPorVolumenStrategy : IDescuentoStrategy
{
    private const decimal UmbralVolumen = 50_000m;
    private const decimal PorcentajeAlto = 0.12m;
    private const decimal PorcentajeBase = 0.05m;

    /// <inheritdoc />
    public string TipoCliente => "PorVolumen";

    /// <inheritdoc />
    public decimal Calcular(decimal montoCompra)
    {
        if (montoCompra <= 0)
            return 0m;

        return montoCompra > UmbralVolumen
            ? montoCompra * PorcentajeAlto
            : montoCompra * PorcentajeBase;
    }
}
