using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Compartido.DTOs.Pago;
using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Pago;
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
    private readonly IRegistrarPagoPresupuesto _registrarPagoPresupuesto;
    private readonly IObtenerPresupuestoConReserva _obtenerPresupuestosConReserva;
    
    public PresupuestoController(ICrearPresupuesto crearPresupuesto, IObtenerPresupuesto obtenerPresupuesto,IRegistrarPagoPresupuesto registrarPagoPresupuesto, IObtenerPresupuestoConReserva obtenerPresupuestosConReserva)
    {
        _crearPresupuesto = crearPresupuesto;
        _obtenerPresupuesto = obtenerPresupuesto;
        _registrarPagoPresupuesto = registrarPagoPresupuesto;
        _obtenerPresupuestosConReserva = obtenerPresupuestosConReserva;
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
    /// Registra un pago asociado a una reserva existente.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite al administrador registrar un pago realizado por un cliente.
    /// Cada pago se guarda de forma independiente, incluyendo el monto y la fecha,
    /// permitiendo mantener un historial completo de pagos asociados a la reserva.
    ///
    /// El pago se asocia al presupuesto vinculado a la reserva indicada.
    /// </remarks>
    /// <param name="idReserva">
    /// Identificador de la reserva a la cual se desea registrar el pago.
    /// </param>
    /// <param name="dto">
    /// Datos del pago a registrar, incluyendo el monto abonado y la fecha del pago.
    /// </param>
    /// <param name="ct">
    /// Token de cancelación para finalizar la operación si la solicitud es cancelada.
    /// </param>
    /// <returns>
    /// Retorna el presupuesto actualizado con el historial de pagos registrado.
    /// </returns>
    /// <response code="200">
    /// El pago fue registrado correctamente y el presupuesto fue actualizado.
    /// </response>
    /// <response code="400">
    /// Los datos enviados son inválidos o incompletos.
    /// </response>
    /// <response code="404">
    /// No se encontró la reserva o el presupuesto asociado.
    /// </response>
    /// <response code="500">
    /// Ocurrió un error inesperado al registrar el pago.
    /// </response>
    [HttpPost("reserva/{idReserva:int}/pagos")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(PresupuestoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegistrarPago([FromRoute] int idReserva, [FromBody] PagoCrearDto dto, CancellationToken ct)
    {
        try
        {
            var actualizado = await _registrarPagoPresupuesto.Registrar(idReserva, dto, ct);
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
                new { message = "Ocurrió un error inesperado al registrar el pago." });
        }
    }

    /// <summary>
    /// Obtiene todos los presupuestos de un usuario, incluyendo la información de la reserva asociada.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite consultar el listado de presupuestos pertenecientes a un usuario.
    /// Cada ítem incluye los datos del presupuesto y la reserva asociada (si corresponde),
    /// listo para ser consumido por el frontend.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Lista de presupuestos obtenida correctamente (puede ser vacía).
    /// - `400 Bad Request` → Id de usuario inválido.
    /// - `500 Internal Server Error` → Error inesperado.
    /// </remarks>
    /// <param name="idUsuario">Identificador del usuario del cual se desean obtener los presupuestos.</param>
    /// <param name="ct">Token de cancelación.</param>
    /// <returns>
    /// Retorna una lista de <see cref="PresupuestoConReservaDto"/> con los presupuestos del usuario y su reserva.
    /// </returns>
    /// <response code="200">Lista obtenida correctamente.</response>
    /// <response code="400">El idUsuario es inválido.</response>
    /// <response code="500">Ocurrió un error inesperado.</response>
    [HttpGet("usuario/{idUsuario:int}/con-reserva")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<PresupuestoConReservaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerPresupuestosConReservaPorUsuario([FromRoute] int idUsuario, CancellationToken ct)
    {
        try
        {
            var presupuestos = await _obtenerPresupuestosConReserva.Ejecutar(idUsuario, ct);
            return Ok(presupuestos ?? Enumerable.Empty<PresupuestoConReservaDto>());
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Ocurrió un error inesperado al obtener los presupuestos del usuario." });
        }
    }
}