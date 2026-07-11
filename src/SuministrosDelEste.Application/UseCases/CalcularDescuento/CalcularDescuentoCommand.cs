namespace SuministrosDelEste.Application.UseCases.CalcularDescuento;

/// <summary>
/// Command de entrada para el caso de uso "Calcular Descuento".
/// Mismo estilo que RegistrarMaterialCommand: DTO inmutable de entrada.
/// </summary>
public sealed record CalcularDescuentoCommand(
    string TipoCliente,
    decimal MontoCompra
);
