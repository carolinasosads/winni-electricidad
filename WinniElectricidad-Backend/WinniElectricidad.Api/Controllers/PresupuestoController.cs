using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;

namespace WinniElectricidad.Api.Controllers;

/// <summary>
/// Controlador que gestiona las operaciones relacionadas a presupuestos.
/// </summary>
[Route("WinniElectricidadApi/[controller]")]
[ApiController]
public class PresupuestoController : ControllerBase
{
    private readonly ICrearPresupuesto _crearPresupuesto;
    private readonly IObtenerPresupuesto _obtenerPresupuesto;
    private readonly IActualizarMontoPagadoPresupuesto _actualizarMontoPagadoPresupuesto;

    public PresupuestoController(ICrearPresupuesto crearPresupuesto, IObtenerPresupuesto obtenerPresupuesto, IActualizarMontoPagadoPresupuesto actualizarMontoPagadoPresupuesto)
    {
        _crearPresupuesto = crearPresupuesto;
        _obtenerPresupuesto = obtenerPresupuesto;
        _actualizarMontoPagadoPresupuesto = actualizarMontoPagadoPresupuesto;
    }

    /// <summary>
    /// Crea un presupuesto para una reserva existente (solo administradores).
    /// </summary>
    /// <remarks>
    /// **Flujo:**
    /// 1. El administrador selecciona una reserva en el panel.  
    /// 2. Envía el id de la reserva en la ruta y los datos del presupuesto en el cuerpo.  
    /// 3. El sistema valida que la reserva exista y que no tenga ya un presupuesto asociado.  
    /// 4. Se crea el presupuesto vinculado a la reserva y al administrador que lo genera.
    ///
    /// **Requiere autenticación:**
    /// - Rol <c>Administrador</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `201 Created` → Presupuesto creado correctamente.  
    /// - `400 Bad Request` → Datos inválidos.  
    /// - `404 Not Found` → La reserva no existe.  
    /// - `409 Conflict` → La reserva ya tiene un presupuesto.  
    /// - `500 Internal Server Error` → Error inesperado.
    /// </remarks>
    /// <param name="idReserva">Identificador de la reserva.</param>
    /// <param name="dto">Datos del presupuesto a crear.</param>
    /// <param name="ct">Token de cancelación.</param>
    [ProducesResponseType(typeof(PresupuestoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("reserva/{idReserva:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearPresupuestoParaReserva([FromRoute] int idReserva, [FromBody] PresupuestoCrearDto dto, CancellationToken ct)
    {
        try
        {
            var presupuestoCreado = await _crearPresupuesto.Ejecutar(idReserva, dto, ct);
            return StatusCode(StatusCodes.Status201Created, presupuestoCreado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocurrió un error inesperado al crear el presupuesto." });
        }
    }
    
    
    
    /// <summary>
    /// Obtiene el presupuesto asociado a una reserva.
    /// </summary>
    /// <remarks>
    /// **Códigos de respuesta:**
    /// - `200 OK` → Presupuesto encontrado.
    /// - `400 Bad Request` → Id de reserva inválido.
    /// - `404 Not Found` → No existe presupuesto para esa reserva.
    /// - `500 Internal Server Error` → Error inesperado.
    /// </remarks>
    [ProducesResponseType(typeof(PresupuestoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("reserva/{idReserva:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerPresupuestoPorReserva(
        [FromRoute] int idReserva,
        CancellationToken ct)
    {
        try
        {
            var presupuesto = await _obtenerPresupuesto.Ejecutar(idReserva, ct);
            return Ok(presupuesto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Ocurrió un error inesperado al obtener el presupuesto." });
        }
    }
    
    /// <summary>
    /// Actualiza el monto pagado del presupuesto asociado a una reserva.
    /// </summary>
    /// <remarks>
    /// **Códigos de respuesta:**
    /// - `200 OK` → Monto pagado actualizado.
    /// - `500 Internal Server Error` → Error inesperado.
    /// </remarks>
    [ProducesResponseType(typeof(PresupuestoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPatch("reserva/{idReserva:int}/monto-pagado")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ActualizarMontoPagado([FromRoute] int idReserva, [FromBody] PresupuestoMontoPagadoActualizarDto dto, CancellationToken ct)
    {
        try
        {
            var actualizado = await _actualizarMontoPagadoPresupuesto.Actualizar(idReserva, dto, ct);
            return Ok(actualizado);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Ocurrió un error inesperado al actualizar el monto pagado del presupuesto." });
        }
    }
}
    
    
    
