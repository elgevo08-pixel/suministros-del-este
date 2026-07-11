namespace SuministrosDelEste.Application.DTOs;

/// <summary>DTO de respuesta con el detalle del descuento calculado.</summary>
public sealed record DescuentoCalculadoDto
{
    /// <summary>Tipo de cliente para el que se calculó el descuento.</summary>
    public string TipoCliente { get; init; } = string.Empty;

    /// <summary>Monto original de la compra, antes de descuento.</summary>
    public decimal MontoCompra { get; init; }

    /// <summary>Monto de descuento calculado.</summary>
    public decimal MontoDescuento { get; init; }

    /// <summary>Monto final a pagar (MontoCompra - MontoDescuento).</summary>
    public decimal MontoFinal { get; init; }
}
