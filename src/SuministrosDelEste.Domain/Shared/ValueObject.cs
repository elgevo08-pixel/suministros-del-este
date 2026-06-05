namespace SuministrosDelEste.Domain.Shared;

/// <summary>
/// Clase base para todos los Value Objects del dominio.
/// Garantiza igualdad por valor (no por referencia) e inmutabilidad.
/// Patrón DDD: Value Object
/// </summary>
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        return GetEqualityComponents()
            .SequenceEqual(((ValueObject)obj).GetEqualityComponents());
    }

    public override int GetHashCode()
        => GetEqualityComponents()
            .Aggregate(0, (hash, component) => HashCode.Combine(hash, component?.GetHashCode() ?? 0));

    public static bool operator ==(ValueObject? left, ValueObject? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(ValueObject? left, ValueObject? right)
        => !(left == right);
}
