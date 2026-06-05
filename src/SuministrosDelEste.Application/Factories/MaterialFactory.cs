using SuministrosDelEste.Application.UseCases.RegistrarMaterial;
using SuministrosDelEste.Domain.Aggregates.Material;
using SuministrosDelEste.Domain.Aggregates.Material.ValueObjects;

namespace SuministrosDelEste.Application.Factories;

/// <summary>
/// Implementación de la Factory de materiales.
/// Traduce el Command primitivo (datos sin validar) al Aggregate Root con Value Objects validados.
///
/// Patrón de diseño: Factory
/// Principio SOLID SRP: única responsabilidad, construir instancias válidas de Material.
/// Principio SOLID OCP: se extiende para nuevas reglas de construcción sin modificar handlers.
/// </summary>
public sealed class MaterialFactory : IMaterialFactory
{
    /// <inheritdoc />
    public Material Crear(RegistrarMaterialCommand command)
    {
        // Construir Value Objects — las validaciones del dominio se ejecutan aquí
        NombreMaterial nombre = NombreMaterial.Crear(command.Nombre);
        Stock stockActual = Stock.Crear(command.StockActual);
        Stock stockMinimo = Stock.Crear(command.StockMinimo);
        Precio precio = Precio.Crear(command.PrecioUnitario);

        // Delegar la creación al Factory Method del Aggregate Root
        return Material.Crear(nombre, command.Descripcion, stockActual, stockMinimo, precio);
    }
}
