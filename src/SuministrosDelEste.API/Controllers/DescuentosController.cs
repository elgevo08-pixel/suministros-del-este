using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuministrosDelEste.Application.DTOs;
using SuministrosDelEste.Application.Ports;
using SuministrosDelEste.Application.UseCases.CalcularDescuento;
using SuministrosDelEste.Domain.Exceptions;

namespace SuministrosDelEste.API.Controllers;

/// <summary>
/// Adaptador primario (Primary/Driving Adapter) REST para el cálculo de descuentos.
/// Ejemplo vivo de la Tarea 4 (SOLID) — ver docs/solid-antes-despues/ para el antes/después completo.
///
/// Depende de ICalcularDescuentoUseCase (abstracción), nunca de CalcularDescuentoHandler
/// directamente — Principio SOLID DIP, mismo patrón que MaterialesController.
///
/// Patrón Hexagonal: Primary Adapter
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public sealed class DescuentosController : ControllerBase
{
    private readonly ICalcularDescuentoUseCase _calcularDescuento;

    public DescuentosController(ICalcularDescuentoUseCase calcularDescuento)
    {
        _calcularDescuento = calcularDescuento;
    }

    /// <summary>
    /// Calcula el descuento aplicable a una compra según el tipo de cliente.
    /// Roles requeridos: cualquier usuario autenticado.
    /// </summary>
    [HttpPost("calcular")]
    [ProducesResponseType(typeof(DescuentoCalculadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Calcular([FromBody] CalcularDescuentoCommand command)
    {
        try
        {
            DescuentoCalculadoDto resultado = await _calcularDescuento.EjecutarAsync(command);
            return Ok(resultado);
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message); // Regla de negocio violada → HTTP 400
        }
    }
}
