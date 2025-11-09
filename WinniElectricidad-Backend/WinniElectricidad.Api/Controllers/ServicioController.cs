using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;

namespace WinniElectricidad.Api.Controllers;

/// <summary>
/// Controlador que gestiona las operaciones de servicios.
/// </summary>
[Route("WinniElectricidadApi/[controller]")]
[ApiController]
public class ServicioController : ControllerBase
{
    private readonly IObtenerServiciosDisponibles _obtenerServiciosDisponibles;
    
    /// <summary>
    /// Inicializa una nueva instancia del <see cref="ServicioController"/> con las dependencias necesarias.
    /// </summary>
    /// <param name="obtenerServiciosDisponibles">Servicio para obtener los servicios activos.</param>
    public ServicioController(IObtenerServiciosDisponibles obtenerServiciosDisponibles)
    {
        _obtenerServiciosDisponibles = obtenerServiciosDisponibles;
    }
  
    /// <summary>
    /// Obtiene la lista de servicios técnicos actualmente disponibles en la plataforma.
    /// </summary>
    /// <remarks>
    /// Este endpoint devuelve todos los servicios activos que pueden ser seleccionados por los usuarios
    /// al momento de realizar una reserva o solicitar un presupuesto.
    ///
    /// **Flujo:**
    /// 1. Consulta la base de datos a través del servicio <see cref="_obtenerServiciosDisponibles"/>.  
    /// 2. Devuelve una colección de objetos <see cref="ServicioDisponibleDto"/> con la información de cada servicio disponible.  
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Lista de servicios obtenida correctamente.  
    /// - `500 Internal Server Error` → Error inesperado al obtener los datos.
    ///
    /// </remarks>
    /// <param name="ct">Token de cancelación para interrumpir la operación si es necesario.</param>
    /// <returns>
    /// Una respuesta HTTP con la colección de servicios disponibles.
    /// </returns>
    /// <response code="200">Lista de servicios obtenida correctamente.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpGet("disponibles")]
    [ProducesResponseType(typeof(IEnumerable<ServicioDisponibleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetServiciosActivos(CancellationToken ct)
    {
        try
        {
            IEnumerable<ServicioDisponibleDto> serviciosDisponibles = await _obtenerServiciosDisponibles.Ejecutar(ct);
            return Ok(serviciosDisponibles);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado."});
        }
    }
}