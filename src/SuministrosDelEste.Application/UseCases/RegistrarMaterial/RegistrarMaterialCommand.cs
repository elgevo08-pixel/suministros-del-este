namespace SuministrosDelEste.Application.UseCases.RegistrarMaterial;

/// <summary>
/// Comando de entrada para el caso de uso RegistrarMaterial.
/// Contiene únicamente tipos primitivos — sin dependencias del dominio.
/// Patrón: Command (CQRS)
/// </summary>
public sealed record RegistrarMaterialCommand(
    string Nombre,
    string? Descripcion,
    decimal StockActual,
    decimal StockMinimo,
    decimal PrecioUnitario
);
