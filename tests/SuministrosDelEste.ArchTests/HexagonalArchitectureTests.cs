using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Xunit;
using ArchUnitNET.xUnit;
using SuministrosDelEste.Application.UseCases.RegistrarMaterial;
using SuministrosDelEste.Domain.Aggregates.Material;
using SuministrosDelEste.Domain.Shared;
using SuministrosDelEste.Infrastructure.Persistence.Context;
using SuministrosDelEste.API.Controllers;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace SuministrosDelEste.ArchTests;

/// <summary>
/// Pruebas de arquitectura con ArchUnitNET.
/// Validan en tiempo de compilación/test que se respetan los límites de la Arquitectura Hexagonal.
/// Un test fallido significa que una capa viola sus restricciones de dependencia.
///
/// Ejecutar con: dotnet test tests/SuministrosDelEste.ArchTests/
/// </summary>
public class HexagonalArchitectureTests
{
    // Carga los ensamblados una sola vez para todas las pruebas de la clase
    private static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Material).Assembly,                  // Domain
            typeof(RegistrarMaterialHandler).Assembly,  // Application
            typeof(AppDbContext).Assembly,               // Infrastructure
            typeof(MaterialesController).Assembly       // API
        )
        .Build();

    // Namespaces raíz de cada capa
    private const string DomainNs        = "SuministrosDelEste.Domain";
    private const string ApplicationNs   = "SuministrosDelEste.Application";
    private const string InfrastructureNs = "SuministrosDelEste.Infrastructure";
    private const string ApiNs           = "SuministrosDelEste.API";

    // -----------------------------------------------------------------------
    // Reglas del Dominio: el núcleo no conoce ninguna capa externa
    // -----------------------------------------------------------------------

    [Fact]
    public void Dominio_NoDependeEn_Aplicacion()
    {
        Types().That().ResideInNamespace(DomainNs, true)
            .Should().NotDependOnAnyTypesThat()
            .ResideInNamespace(ApplicationNs, true)
            .Because("El Dominio es el núcleo de la arquitectura hexagonal y no conoce la Aplicación.")
            .Check(Architecture);
    }

    [Fact]
    public void Dominio_NoDependeEn_Infraestructura()
    {
        Types().That().ResideInNamespace(DomainNs, true)
            .Should().NotDependOnAnyTypesThat()
            .ResideInNamespace(InfrastructureNs, true)
            .Because("El Dominio define los puertos; la Infraestructura implementa los adaptadores.")
            .Check(Architecture);
    }

    [Fact]
    public void Dominio_NoDependeEn_API()
    {
        Types().That().ResideInNamespace(DomainNs, true)
            .Should().NotDependOnAnyTypesThat()
            .ResideInNamespace(ApiNs, true)
            .Because("El Dominio no debe conocer la capa de Presentación.")
            .Check(Architecture);
    }

    // -----------------------------------------------------------------------
    // Reglas de la Aplicación: orquesta, pero no conoce la Infraestructura
    // -----------------------------------------------------------------------

    [Fact]
    public void Aplicacion_NoDependeEn_Infraestructura()
    {
        Types().That().ResideInNamespace(ApplicationNs, true)
            .Should().NotDependOnAnyTypesThat()
            .ResideInNamespace(InfrastructureNs, true)
            .Because("La Aplicación depende solo de puertos (interfaces), no de adaptadores concretos.")
            .Check(Architecture);
    }

    [Fact]
    public void Aplicacion_NoDependeEn_API()
    {
        Types().That().ResideInNamespace(ApplicationNs, true)
            .Should().NotDependOnAnyTypesThat()
            .ResideInNamespace(ApiNs, true)
            .Because("La capa de Aplicación no debe conocer la capa de Presentación.")
            .Check(Architecture);
    }

    // -----------------------------------------------------------------------
    // Reglas de DDD: Value Objects deben heredar de ValueObject
    // -----------------------------------------------------------------------

    [Fact]
    public void ValueObjects_DebenHeredarDeValueObjectBase()
    {
        Classes()
            .That().ResideInNamespace("SuministrosDelEste.Domain.Aggregates.Material.ValueObjects", true)
            .Should().BeAssignableTo(typeof(ValueObject))
            .Because("Todo tipo en el namespace ValueObjects es un Value Object de dominio.")
            .Check(Architecture);
    }
}
