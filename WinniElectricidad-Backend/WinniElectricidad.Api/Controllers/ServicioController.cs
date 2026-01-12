using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Api.Servicios;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Servicios;

namespace WinniElectricidad.Api.Controllers;

/// <summary>
/// Controlador que gestiona las operaciones de servicios.
/// </summary>
[Route("WinniElectricidadApi/[controller]")]
[ApiController]
public class ServicioController : ControllerBase
{
    private readonly IObtenerServiciosSegunEstado _obtenerServiciosSegunEstado;
    private readonly IDesactivarServicio _desactivarServicio;
    private readonly IActivarServicio _activarServicio;
    private readonly ICrearServicio _crearServicio;
    private readonly IEditarServicio _editarServicio;

    private readonly IServicioImagenes _servicioImagenes;

    /// <summary>
    /// Inicializa una nueva instancia del <see cref="ServicioController"/> con las dependencias necesarias.
    /// </summary>
    /// <param name="obtenerServiciosSegunEstado">Servicio para obtener los servicios activos.</param>
    /// <param name="desactivarServicio">Servicio para desactivar un servicio activo.</param>
    /// <param name="activarServicio">Servicio para activar un servicio desactivado.</param>
    /// <param name="crearServicio">Servicio para crear un nuevo servicio ofrecido por la empresa.</param>
    /// <param name="editarServicio">Servicio para editar un servicio existente.</param>
    /// <param name="servicioImagenes">Servicio para guardar o remover imagenes en Azure blob.</param>
    public ServicioController(IObtenerServiciosSegunEstado obtenerServiciosSegunEstado, IDesactivarServicio desactivarServicio, IActivarServicio activarServicio, ICrearServicio crearServicio, IEditarServicio editarServicio, IServicioImagenes servicioImagenes)
    {
        _obtenerServiciosSegunEstado = obtenerServiciosSegunEstado;
        _desactivarServicio = desactivarServicio;
        _activarServicio = activarServicio;
        _crearServicio = crearServicio;
        _editarServicio = editarServicio;
        _servicioImagenes = servicioImagenes;
    }
    
    /// <summary>
    /// Obtiene la lista de servicios técnicos actualmente activos en la plataforma.
    /// </summary>
    /// <remarks>
    /// Este endpoint devuelve todos los servicios activos que pueden ser seleccionados por los usuarios
    /// al momento de realizar una reserva o solicitar un presupuesto.
    ///
    /// **Flujo:**
    /// 1. Consulta la base de datos a través del servicio <see cref="_obtenerServiciosSegunEstado"/>.  
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
            IEnumerable<ServicioActivoDto> serviciosDisponibles = await _obtenerServiciosSegunEstado.Ejecutar(true, ct);
            return Ok(serviciosDisponibles);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado."});
        }
    }
    
    /// <summary>
    /// Obtiene toda la lista de servicios técnicos.
    /// </summary>
    /// <remarks>
    /// Este endpoint devuelve todos los servicios para que el administrador pueda activarlos o desactivarlos.
    ///
    /// **Flujo:**
    /// 1. Consulta la base de datos a través del servicio <see cref="_obtenerServiciosSegunEstado"/>.  
    /// 2. Devuelve una colección de objetos <see cref="ServicioActivoDto"/> con la información de cada servicio disponible.  
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Lista de servicios obtenida correctamente.  
    /// - `500 Internal Server Error` → Error inesperado al obtener los datos.
    ///
    /// </remarks>
    /// <param name="ct">Token de cancelación para interrumpir la operación si es necesario.</param>
    /// <returns>
    /// Una respuesta HTTP con la colección de servicios.
    /// </returns>
    /// <response code="200">Lista de servicios obtenida correctamente.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ServicioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> GetServicios(CancellationToken ct)
    {
        try
        {
            var servicios = await _obtenerServiciosSegunEstado.Ejecutar(false, ct);
            return Ok(servicios);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado."});
        }
    }
    
    /// <summary>
    /// Desactiva un servicio existente del sistema.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite desactivar un servicio identificado por su ID, evitando que
    /// pueda ser seleccionado o utilizado en nuevas reservas.
    ///
    /// La operación no elimina el servicio, únicamente cambia su estado a inactivo.
    ///
    /// **Requiere autenticación:**  
    /// - Solo disponible para usuarios con el rol <c>Administrador</c>.
    ///
    /// **Flujo:**
    /// 1. Recibe el identificador del servicio como parámetro de ruta.  
    /// 2. Ejecuta la lógica de desactivación mediante el servicio <see cref="_desactivarServicio"/>.  
    /// 3. Devuelve una confirmación si la operación fue exitosa.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Servicio desactivado correctamente.  
    /// - `400 Bad Request` → El servicio no existe o ya se encuentra desactivado.  
    /// - `500 Internal Server Error` → Error inesperado del servidor.
    /// </remarks>
    /// <param name="idServicio">
    /// Identificador único del servicio que se desea desactivar.
    /// </param>
    /// <param name="ct">
    /// Token de cancelación para abortar la operación si es necesario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP que confirma la desactivación del servicio.
    /// </returns>
    /// <response code="200">Servicio desactivado correctamente.</response>
    /// <response code="400">El servicio no existe o no puede ser desactivado.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [Authorize(Roles = "Administrador")]
    [HttpPatch("desactivar/{idServicio}")]
    public async Task<IActionResult> DesactivarServicio([FromRoute] int idServicio, CancellationToken ct)
    {
        try
        {
            await _desactivarServicio.Ejecutar(idServicio, ct);
            return Ok(new { message = "Servicio desactivado con éxito." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }
    
    /// <summary>
    /// Activa un servicio previamente desactivado.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite reactivar un servicio identificado por su ID,
    /// habilitándolo nuevamente para ser utilizado en nuevas reservas.
    ///
    /// La operación no crea un nuevo servicio, únicamente cambia su estado a activo.
    ///
    /// **Requiere autenticación:**  
    /// - Solo disponible para usuarios con el rol <c>Administrador</c>.
    ///
    /// **Flujo:**
    /// 1. Recibe el identificador del servicio como parámetro de ruta.  
    /// 2. Ejecuta la lógica de activación mediante el servicio <see cref="_activarServicio"/>.  
    /// 3. Devuelve una confirmación si la operación fue exitosa.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Servicio activado correctamente.  
    /// - `400 Bad Request` → El servicio no existe o ya se encuentra activo.  
    /// - `500 Internal Server Error` → Error inesperado del servidor.
    /// </remarks>
    /// <param name="idServicio">
    /// Identificador único del servicio que se desea activar.
    /// </param>
    /// <param name="ct">
    /// Token de cancelación para abortar la operación si es necesario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP que confirma la activación del servicio.
    /// </returns>
    /// <response code="200">Servicio activado correctamente.</response>
    /// <response code="400">El servicio no existe o no puede ser activado.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [Authorize(Roles = "Administrador")]
    [HttpPatch("activar/{idServicio}")]
    public async Task<IActionResult> ActivarServicio([FromRoute] int idServicio, CancellationToken ct)
    {
        try
        {
            await _activarServicio.Ejecutar(idServicio, ct);
            return Ok(new { message = "Servicio activado con éxito." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ServicioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CrearNuevoServicio([FromForm] CrearServicioDto nuevoServicio, ICollection<IFormFile> imagenes, CancellationToken cancellationToken)
    {
        ICollection<string>? imagenesUrl = null;
        
        try
        {
            imagenesUrl = await _servicioImagenes.GuardarImagenesAsync(imagenes, nuevoServicio.Titulo , "servicios");
            
            var servicioCreado = await _crearServicio.Ejecutar(nuevoServicio, imagenesUrl, cancellationToken);
            
            return Ok(servicioCreado);
        } catch (ServicioException ex)
        {
            await EliminarImagenes(imagenesUrl);
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            await EliminarImagenes(imagenesUrl);
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            await EliminarImagenes(imagenesUrl);
            return Forbid(ex.Message);
        }
        catch (Exception)
        {
            await EliminarImagenes(imagenesUrl);
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }

    [HttpPut("{idServicio}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Editar(int idServicio, [FromForm] EditarServicioDto servicio, [FromForm(Name = "imagenes")] List<IFormFile>? imagenesNuevas, CancellationToken cancellationToken)
    {
        ICollection<string>? imagenesUrl = null;
        
        try
        {
            if (imagenesNuevas is { Count: > 0 })
                imagenesUrl = await _servicioImagenes.GuardarImagenesAsync(imagenesNuevas, servicio.Titulo , "servicios");
            
            var servicioEditado = await _editarServicio.Ejecutar(idServicio, servicio, imagenesUrl, cancellationToken);
            
            return Ok(servicioEditado);
        } catch (ServicioException ex)
        {
            await EliminarImagenes(imagenesUrl);
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            await EliminarImagenes(imagenesUrl);
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            await EliminarImagenes(imagenesUrl);
            return Forbid(ex.Message);
        }
        catch (Exception)
        {
            await EliminarImagenes(imagenesUrl);
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }
    private async Task EliminarImagenes(ICollection<string>? urls)
    {
        if (urls is { Count: > 0 })
        {
            await _servicioImagenes.EliminarImagenesAsync(urls);
        }
    }
}