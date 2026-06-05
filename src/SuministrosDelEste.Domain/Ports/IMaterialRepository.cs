namespace SuministrosDelEste.Domain.Ports;

/// <summary>
/// Puerto de salida (Outbound Port / Secondary Port) para la persistencia de materiales.
/// Definido en el Dominio; implementado por la capa de Infraestructura.
///
/// Patrón Hexagonal: Secondary Port (el Dominio define lo que necesita de afuera)
/// Patrón: Repository
/// Principio SOLID DIP: el Dominio depende de la abstracción, no del proveedor de BD.
/// </summary>
public interface IMaterialRepository
{
    /// <summary>Retorna todos los materiales activos, ordenados por nombre.</summary>
    Task<IEnumerable<Aggregates.Material.Material>> ObtenerTodosAsync();

    /// <summary>Busca un material por su ID. Retorna null si no existe o está inactivo.</summary>
    Task<Aggregates.Material.Material?> ObtenerPorIdAsync(int id);

    /// <summary>Persiste un nuevo material y retorna la entidad con el ID asignado por BD.</summary>
    Task<Aggregates.Material.Material> GuardarAsync(Aggregates.Material.Material material);

    /// <summary>Actualiza un material existente y retorna la entidad actualizada.</summary>
    Task<Aggregates.Material.Material> ActualizarAsync(Aggregates.Material.Material material);

    /// <summary>Desactiva lógicamente un material (IsActive = false). Nunca DELETE físico.</summary>
    Task<bool> DesactivarAsync(int id);

    /// <summary>
    /// Retorna materiales activos cuyo StockActual es menor o igual al StockMinimo.
    /// Fuente de datos para el módulo de alertas de reabastecimiento.
    /// </summary>
    Task<IEnumerable<Aggregates.Material.Material>> ObtenerBajoStockMinimoAsync();
}
