using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

namespace WinniElectricidad.Api.Controllers;

/// <summary>
/// Controlador que gestiona las operaciones de reserva de presupuesto.
/// </summary>
[Route("WinniElectricidadApi/[controller]")]
[ApiController]
public class ReservaController : ControllerBase
{
    private readonly IObtenerHorariosDisponibles _obtenerHorariosDisponibles;
    /// <summary>
    /// Inicializa una nueva instancia del <see cref="ReservaController"/> con las dependencias necesarias.
    /// </summary>
    public ReservaController(IObtenerHorariosDisponibles obtenerHorariosDisponibles)
    {
        _obtenerHorariosDisponibles = obtenerHorariosDisponibles;
    }
    
    /// <summary>
    /// Obtiene la disponibilidad de horarios para la solicitud de presupuestos o reservas.
    /// </summary>
    /// <remarks>
    /// Este endpoint devuelve los días y horarios disponibles para que el usuario autenticado pueda agendar una cita de presupuesto.
    /// 
    /// **Flujo:**
    /// 1. Consulta la disponibilidad configurada en el sistema mediante el servicio <see cref="IObtenerHorariosDisponibles"/>.  
    /// 2. Devuelve una colección de objetos <see cref="DiaDisponibilidadDto"/> que representan los días y horarios habilitados.  
    ///
    /// **Requiere autenticación:**  
    /// - Solo disponible para usuarios con el rol <c>Cliente</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Lista de horarios disponibles obtenida correctamente.  
    /// - `500 Internal Server Error` → Error inesperado del servidor.
    /// </remarks>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite interrumpir la operación si es necesario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP que contiene la lista de días y horarios disponibles para reservas.
    /// </returns>
    /// <response code="200">Lista de horarios disponibles obtenida correctamente.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpGet("disponibilidad")]
    [ProducesResponseType(typeof(IEnumerable<DiaDisponibilidadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> GetHorariosDisponibles(CancellationToken cancellationToken)
    {
        try
        {
            var horariosDisponibles = await _obtenerHorariosDisponibles.Ejecutar(cancellationToken);
            return Ok(horariosDisponibles);
        } catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }
}