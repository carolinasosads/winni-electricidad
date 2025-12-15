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

    public PresupuestoController(ICrearPresupuesto crearPresupuesto)
    {
        _crearPresupuesto = crearPresupuesto;
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
}