using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuministrosDelEste.Application.DTOs;
using SuministrosDelEste.Application.Ports;
using SuministrosDelEste.Application.UseCases.ConsultarInventario;
using SuministrosDelEste.Application.UseCases.RegistrarMaterial;
using SuministrosDelEste.Domain.Exceptions;

namespace SuministrosDelEste.API.Controllers;

/// <summary>
/// Adaptador primario (Primary / Driving Adapter) REST para el módulo de Inventario.
/// Recibe peticiones HTTP y las delega a los Inbound Ports de la capa de Aplicación.
///
/// Patrón Hexagonal: Primary Adapter — el exterior interactúa con la aplicación vía este controlador.
/// Seguridad: todos los endpoints requieren token JWT válido emitido por Keycloak.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize] // Requiere token JWT válido de Keycloak para cualquier endpoint
public sealed class MaterialesController : ControllerBase
{
    private readonly IRegistrarMaterialUseCase _registrarMaterial;
    private readonly IConsultarInventarioUseCase _consultarInventario;

    public MaterialesController(
        IRegistrarMaterialUseCase registrarMaterial,
        IConsultarInventarioUseCase consultarInventario)
    {
        _registrarMaterial = registrarMaterial;
        _consultarInventario = consultarInventario;
    }

    /// <summary>
    /// Obtiene el catálogo completo de materiales activos.
    /// Roles requeridos: cualquier usuario autenticado en Keycloak.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MaterialDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObtenerTodos()
    {
        IEnumerable<MaterialDto> materiales = await _consultarInventario
            .ObtenerTodosAsync(new ConsultarInventarioQuery());

        return Ok(materiales);
    }

    /// <summary>
    /// Obtiene materiales con stock en nivel crítico o agotado.
    /// Roles requeridos: cualquier usuario autenticado.
    /// </summary>
    [HttpGet("alertas")]
    [ProducesResponseType(typeof(IEnumerable<AlertaReabastecimientoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObtenerAlertas()
    {
        IEnumerable<AlertaReabastecimientoDto> alertas = await _consultarInventario
            .ObtenerAlertasAsync(new ConsultarInventarioQuery());

        return Ok(alertas);
    }

    /// <summary>
    /// Registra un nuevo material en el inventario.
    /// Roles requeridos: inventario-admin (configurado en Keycloak realm "suministros").
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "inventario-admin")]
    [ProducesResponseType(typeof(MaterialDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarMaterialCommand command)
    {
        try
        {
            MaterialDto resultado = await _registrarMaterial.EjecutarAsync(command);
            return CreatedAtAction(nameof(ObtenerTodos), new { id = resultado.MaterialId }, resultado);
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message); // Regla de negocio violada → HTTP 400
        }
    }
}
