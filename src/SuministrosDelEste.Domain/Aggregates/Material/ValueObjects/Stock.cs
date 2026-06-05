using SuministrosDelEste.Domain.Exceptions;
using SuministrosDelEste.Domain.Shared;

namespace SuministrosDelEste.Domain.Aggregates.Material.ValueObjects;

/// <summary>
/// Value Object: representa una cantidad de stock en unidades decimales.
/// Admite decimales para materiales vendidos por m³, quintales o toneladas.
/// Patrón DDD: Value Object
/// </summary>
public sealed class Stock : ValueObject
{
    public decimal Valor { get; }

    private Stock(decimal valor) => Valor = valor;

    /// <summary>
    /// Fábrica validante: rechaza valores negativos.
    /// </summary>
    public static Stock Crear(decimal valor)
    {
        if (valor < 0)
            throw new DomainException($"El stock no puede ser negativo. Valor recibido: {valor}.");

        return new Stock(valor);
    }

    /// <summary>Retorna true si este stock es menor o igual al stock de referencia.</summary>
    public bool EsMenorOIgualQue(Stock otro) => Valor <= otro.Valor;

    /// <summary>Retorna true si el stock está completamente agotado.</summary>
    public bool EsAgotado() => Valor == 0m;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Valor;
    }

    public override string ToString() => $"{Valor:F4}";
}
