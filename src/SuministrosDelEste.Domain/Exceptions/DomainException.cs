namespace SuministrosDelEste.Domain.Exceptions;

/// <summary>
/// Excepción base para violaciones de invariantes y reglas de negocio del dominio.
/// Se propaga sin envolver hacia capas superiores para tratamiento diferenciado (HTTP 400).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}
