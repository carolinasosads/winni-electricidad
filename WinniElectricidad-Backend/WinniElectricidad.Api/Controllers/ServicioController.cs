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
    private readonly IObtenerServiciosActivos _obtenerServiciosActivos;
    
    /// <summary>
    /// Inicializa una nueva instancia del <see cref="ServicioController"/> con las dependencias necesarias.
    /// </summary>
    /// <param name="obtenerServiciosActivos">Servicio para obtener los servicios activos.</param>
    public ServicioController(IObtenerServiciosActivos obtenerServiciosActivos)
    {
        _obtenerServiciosActivos = obtenerServiciosActivos;
    }
  
    /// <summary>
    /// Obtiene la lista de servicios técnicos actualmente activos en la plataforma.
    /// </summary>
    /// <remarks>
    /// Este endpoint devuelve todos los servicios activos que pueden ser seleccionados por los usuarios
    /// al momento de realizar una reserva o solicitar un presupuesto.
    ///
    /// **Flujo:**
    /// 1. Consulta la base de datos a través del servicio <see cref="_obtenerServiciosActivos"/>.  
    /// 2. Devuelve una colección de objetos <see cref="ServicioActivoDto"/> con la información de cada servicio disponible.  
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Lista de servicios obtenida correctamente.  
    /// - `500 Internal Server Error` → Error inesperado al obtener los datos.
    ///
    /// </remarks>
    /// <param name="ct">Token de cancelación para interrumpir la operación si es necesario.</param>
    /// <returns>
    /// Una respuesta HTTP con la colección de servicios activos.
    /// </returns>
    /// <response code="200">Lista de servicios obtenida correctamente.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpGet("activos")]
    [ProducesResponseType(typeof(IEnumerable<ServicioActivoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetServiciosDisponibles(CancellationToken ct)
    {
        try
        {
            IEnumerable<ServicioActivoDto> serviciosDisponibles = await _obtenerServiciosActivos.Ejecutar(ct);
            return Ok(serviciosDisponibles);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado."});
        }
    }
}