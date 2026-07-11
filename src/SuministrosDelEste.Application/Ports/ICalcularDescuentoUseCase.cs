using SuministrosDelEste.Application.DTOs;
using SuministrosDelEste.Application.UseCases.CalcularDescuento;

namespace SuministrosDelEste.Application.Ports;

/// <summary>
/// Puerto de entrada (Inbound Port) del caso de uso "Calcular Descuento".
/// El adaptador REST (DescuentosController) depende de esta abstracción,
/// nunca de CalcularDescuentoHandler directamente — Principio SOLID DIP.
/// </summary>
public interface ICalcularDescuentoUseCase
{
    /// <summary>Calcula el descuento aplicable y retorna el detalle del cálculo.</summary>
    Task<DescuentoCalculadoDto> EjecutarAsync(CalcularDescuentoCommand command);
}
