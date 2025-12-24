using System.Security.Claims;
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

    public PresupuestoController(ICrearPresupuesto crearPresupuesto, IObtenerPresupuesto obtenerPresupuesto, IActualizarMontoPagadoPresupuesto actualizarMontoPagado)
    {
        _crearPresupuesto = crearPresupuesto;
        _obtenerPresupuesto = obtenerPresupuesto;
        _actualizarMontoPagadoPresupuesto = actualizarMontoPagado;
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
    /// Obtiene el presupuesto asociado a una reserva específica.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite a un administrador consultar el presupuesto vinculado a una reserva existente.
    /// Si la reserva no tiene un presupuesto asociado, se retorna un código 404.
    /// </remarks>
    /// <param name="idReserva">
    /// Identificador único de la reserva para la cual se desea obtener el presupuesto.
    /// </param>
    /// <param name="ct">
    /// Token de cancelación para abortar la operación si la solicitud es cancelada.
    /// </param>
    /// <returns>
    /// Retorna un <see cref="PresupuestoDto"/> con la información del presupuesto asociado a la reserva.
    /// </returns>
    /// <response code="200">
    /// Presupuesto obtenido correctamente.
    /// </response>
    /// <response code="404">
    /// La reserva no existe o no tiene un presupuesto asociado.
    /// </response>
    /// <response code="500">
    /// Ocurrió un error inesperado al intentar obtener el presupuesto.
    /// </response>
    [HttpGet("reserva/{idReserva:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(PresupuestoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerPresupuestoPorReserva([FromRoute] int idReserva, CancellationToken ct)
    {
        try
        {
            var dto = await _obtenerPresupuesto.Ejecutar(idReserva, ct);
            if (dto == null)
                return NotFound(new { message = "La reserva no tiene presupuesto asociado." });

            return Ok(dto);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Ocurrió un error inesperado al obtener el presupuesto." });
        }
    }
    
    /// <summary>
    /// Actualiza el monto pagado de un presupuesto asociado a una reserva (solo administradores).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Actualiza el campo <c>MontoPagado</c> del presupuesto vinculado a la reserva indicada.
    /// </para>
    ///
    /// <para><b>Reglas:</b></para>
    /// <list type="bullet">
    ///   <item><description>El <c>idReserva</c> debe ser mayor a 0.</description></item>
    ///   <item><description>El body es requerido.</description></item>
    ///   <item><description><c>MontoPagado</c> no puede ser negativo.</description></item>
    ///   <item><description><c>MontoPagado</c> no puede superar el monto total del presupuesto.</description></item>
    /// </list>
    ///
    /// <para><b>Requiere autenticación:</b> Rol <c>Administrador</c>.</para>
    ///
    /// <para><b>Códigos de respuesta:</b></para>
    /// <list type="bullet">
    ///   <item><description><c>200 OK</c> Monto pagado actualizado correctamente.</description></item>
    ///   <item><description><c>400 Bad Request</c> Datos inválidos o violación de reglas.</description></item>
    ///   <item><description><c>404 Not Found</c> No existe presupuesto para esa reserva.</description></item>
    ///   <item><description><c>500 Internal Server Error</c> Error inesperado.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="idReserva">Identificador de la reserva a la que pertenece el presupuesto.</param>
    /// <param name="dto">DTO con el monto pagado a actualizar.</param>
    /// <param name="ct">Token de cancelación.</param>
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
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = ex.Message });
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
                new { message = "Ocurrió un error inesperado al actualizar el monto pagado del presupuesto." });
        }
    }

}