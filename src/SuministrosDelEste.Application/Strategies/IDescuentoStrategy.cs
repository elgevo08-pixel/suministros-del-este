namespace SuministrosDelEste.Application.Strategies;

/// <summary>
/// Estrategia intercambiable para calcular el descuento aplicable a una compra.
/// Mismo patrón ya usado en IStockAlertStrategy — no se inventó uno nuevo.
///
/// Principio SOLID OCP: agregar un tipo de descuento nuevo es agregar una clase
/// nueva que implemente esta interfaz y registrarla en el DI (Program.cs); ninguna
/// estrategia existente, ni el orquestador (CalcularDescuentoHandler), se modifican.
///
/// Principio SOLID ISP: interfaz mínima — un solo método de cálculo. Ninguna
/// implementación se ve obligada a tener métodos de auditoría, reportes o
/// exportación que no le correspondan (contraste con IDescuentoOperations
/// en docs/solid-antes-despues/antes/).
///
/// Principio SOLID LSP: el contrato es el mismo para todas las implementaciones
/// (ver comentario en Calcular) — es precisamente ese contrato compartido lo que
/// hace que cualquier IDescuentoStrategy sea sustituible por otra sin sorpresas.
/// </summary>
public interface IDescuentoStrategy
{
    /// <summary>Tipo de cliente que esta estrategia representa (ej. "Premium").</summary>
    string TipoCliente { get; }

    /// <summary>
    /// Calcula el descuento para un monto de compra dado.
    /// Contrato que TODA implementación debe cumplir (garantiza LSP):
    ///   - Nunca lanza excepciones para un montoCompra ≥ 0.
    ///   - Siempre retorna un valor entre 0 y montoCompra (nunca negativo,
    ///     nunca mayor que la compra misma).
    /// </summary>
    decimal Calcular(decimal montoCompra);
}
