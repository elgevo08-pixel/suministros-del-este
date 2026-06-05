using SuministrosDelEste.Application.UseCases.RegistrarMaterial;
using SuministrosDelEste.Domain.Aggregates.Material;

namespace SuministrosDelEste.Application.Factories;

/// <summary>
/// Contrato de la Factory de materiales.
/// Patrón de diseño: Factory — desacopla la construcción del Aggregate de su consumidor.
/// Principio SOLID DIP: el handler depende de la abstracción, no de la implementación concreta.
/// </summary>
public interface IMaterialFactory
{
    /// <summary>
    /// Construye un Aggregate Root Material validado a partir de un Command primitivo.
    /// </summary>
    Material Crear(RegistrarMaterialCommand command);
}
