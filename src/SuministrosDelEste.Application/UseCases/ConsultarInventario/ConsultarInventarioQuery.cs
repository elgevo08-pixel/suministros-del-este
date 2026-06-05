namespace SuministrosDelEste.Application.UseCases.ConsultarInventario;

/// <summary>
/// Objeto de consulta para el caso de uso ConsultarInventario.
/// Patrón: Query (CQRS)
/// </summary>
public sealed record ConsultarInventarioQuery(
    bool SoloActivos = true
);
