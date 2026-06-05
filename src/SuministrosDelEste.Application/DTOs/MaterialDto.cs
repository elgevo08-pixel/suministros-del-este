using SuministrosDelEste.Domain.Aggregates.Material;

namespace SuministrosDelEste.Application.DTOs;

/// <summary>
/// Data Transfer Object para la representación de materiales en la capa de presentación.
/// Desacopla el Aggregate Root del dominio del contrato de la API REST.
/// </summary>
public sealed record MaterialDto
{
    public int MaterialId { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public decimal StockActual { get; init; }
    public decimal StockMinimo { get; init; }
    public decimal PrecioUnitario { get; init; }
    public DateTime FechaRegistro { get; init; }
    public bool IsActive { get; init; }
    public bool RequiereReabastecimiento { get; init; }

    /// <summary>
    /// Mapea un Aggregate Root Material a su representación DTO.
    /// </summary>
    public static MaterialDto DesdeEntidad(Material material) => new()
    {
        MaterialId = material.MaterialId,
        Nombre = material.Nombre.Valor,
        Descripcion = material.Descripcion,
        StockActual = material.StockActual.Valor,
        StockMinimo = material.StockMinimo.Valor,
        PrecioUnitario = material.PrecioUnitario.Valor,
        FechaRegistro = material.FechaRegistro,
        IsActive = material.IsActive,
        RequiereReabastecimiento = material.StockActual.EsMenorOIgualQue(material.StockMinimo)
    };
}
