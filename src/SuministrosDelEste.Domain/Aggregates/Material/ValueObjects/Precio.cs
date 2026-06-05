using SuministrosDelEste.Domain.Exceptions;
using SuministrosDelEste.Domain.Shared;

namespace SuministrosDelEste.Domain.Aggregates.Material.ValueObjects;

/// <summary>
/// Value Object: representa el precio unitario de un material.
/// Configurado para precisión monetaria (decimal 18,2).
/// Patrón DDD: Value Object
/// </summary>
public sealed class Precio : ValueObject
{
    public decimal Valor { get; }

    private Precio(decimal valor) => Valor = valor;

    /// <summary>
    /// Fábrica validante: rechaza precios negativos.
    /// </summary>
    public static Precio Crear(decimal valor)
    {
        if (valor < 0)
            throw new DomainException($"El precio unitario no puede ser negativo. Valor recibido: {valor}.");

        return new Precio(valor);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => $"{Valor:C2}";
}
