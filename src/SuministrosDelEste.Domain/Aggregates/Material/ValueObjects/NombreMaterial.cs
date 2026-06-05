using SuministrosDelEste.Domain.Exceptions;
using SuministrosDelEste.Domain.Shared;

namespace SuministrosDelEste.Domain.Aggregates.Material.ValueObjects;

/// <summary>
/// Value Object: encapsula y valida el nombre de un material de construcción.
/// Garantiza que nunca sea nulo, vacío ni supere los 100 caracteres.
/// Patrón DDD: Value Object — igualdad por valor, inmutable.
/// </summary>
public sealed class NombreMaterial : ValueObject
{
    public string Valor { get; }

    private NombreMaterial(string valor) => Valor = valor;

    /// <summary>
    /// Fábrica validante: única vía para crear una instancia.
    /// Lanza <see cref="DomainException"/> si el valor viola las reglas del dominio.
    /// </summary>
    public static NombreMaterial Crear(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("El nombre del material es obligatorio.");

        if (valor.Trim().Length > 100)
            throw new DomainException("El nombre del material no puede superar los 100 caracteres.");

        return new NombreMaterial(valor.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor.ToUpperInvariant(); // Comparación insensible a mayúsculas
    }

    public override string ToString() => Valor;
}
