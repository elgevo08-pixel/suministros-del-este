using SuministrosDelEste.Domain.Aggregates.Material.ValueObjects;
using SuministrosDelEste.Domain.Events;
using SuministrosDelEste.Domain.Exceptions;
using SuministrosDelEste.Domain.Shared;

namespace SuministrosDelEste.Domain.Aggregates.Material;

/// <summary>
/// Aggregate Root del módulo de Inventario.
/// Encapsula y protege todas las reglas de negocio y el estado del material.
///
/// Patrones aplicados:
///   - DDD Aggregate Root: punto de entrada único al agregado
///   - DDD Value Objects: NombreMaterial, Stock, Precio
///   - DDD Domain Events: MaterialRegistradoEvent, StockBajoMinimoEvent
///   - Factory Method: Material.Crear(...)
///   - Observer: Domain Events vía AddDomainEvent(...)
///
/// Principios SOLID:
///   - SRP: única responsabilidad, gestionar el ciclo de vida del material
///   - OCP: extensible por métodos de dominio, no se modifica para nuevas reglas
/// </summary>
public sealed class Material : AggregateRoot
{
    // EF Core utiliza reflexión para establecer propiedades con private set
    public int MaterialId { get; private set; }
    public NombreMaterial Nombre { get; private set; } = null!;
    public string? Descripcion { get; private set; }
    public Stock StockActual { get; private set; } = null!;
    public Stock StockMinimo { get; private set; } = null!;
    public Precio PrecioUnitario { get; private set; } = null!;
    public DateTime FechaRegistro { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>Constructor privado requerido por EF Core para materialización desde BD.</summary>
    private Material() { }

    // -----------------------------------------------------------------------
    // Factory Method del dominio
    // -----------------------------------------------------------------------

    /// <summary>
    /// Única vía válida para crear un Material nuevo.
    /// Aplica invariantes del dominio y eleva los Domain Events correspondientes.
    /// Patrón: Factory Method
    /// </summary>
    public static Material Crear(
        NombreMaterial nombre,
        string? descripcion,
        Stock stockActual,
        Stock stockMinimo,
        Precio precioUnitario)
    {
        Material material = new()
        {
            Nombre = nombre,
            Descripcion = descripcion,
            StockActual = stockActual,
            StockMinimo = stockMinimo,
            PrecioUnitario = precioUnitario,
            FechaRegistro = DateTime.UtcNow,
            IsActive = true
        };

        // Elevar evento: material registrado (Observer Pattern)
        material.AddDomainEvent(new MaterialRegistradoEvent(nombre.Valor, precioUnitario.Valor));

        // Elevar alerta si el material inicia con stock crítico
        if (stockActual.EsMenorOIgualQue(stockMinimo))
            material.AddDomainEvent(
                new StockBajoMinimoEvent(nombre.Valor, stockActual.Valor, stockMinimo.Valor));

        return material;
    }

    // -----------------------------------------------------------------------
    // Comportamiento del dominio
    // -----------------------------------------------------------------------

    /// <summary>
    /// Actualiza el nivel de stock. Genera alerta si cae al nivel mínimo o por debajo.
    /// </summary>
    public void ActualizarStock(Stock nuevoStock)
    {
        StockActual = nuevoStock;

        if (nuevoStock.EsMenorOIgualQue(StockMinimo))
            AddDomainEvent(
                new StockBajoMinimoEvent(Nombre.Valor, nuevoStock.Valor, StockMinimo.Valor));
    }

    /// <summary>
    /// Desactiva lógicamente el material del catálogo.
    /// Nunca se elimina físicamente — eliminación lógica obligatoria.
    /// </summary>
    public void Desactivar()
    {
        if (!IsActive)
            throw new DomainException($"El material '{Nombre.Valor}' ya se encuentra inactivo.");

        IsActive = false;
    }
}
