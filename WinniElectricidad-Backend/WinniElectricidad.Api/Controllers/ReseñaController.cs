using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Api.Servicios;
using WinniElectricidad.Compartido.DTOs.Reseñas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reseñas;

namespace WinniElectricidad.Api.Controllers;

/// <summary>
/// Controlador que gestiona las operaciones de reseñas.
/// </summary>
[Route("WinniElectricidadApi/Resena")]
[ApiController]
public class ReseñaController : ControllerBase
{
    private readonly  IAgregarReseña _agregarReseña;
    private readonly  IServicioImagenes _servicioImagenes;
    private readonly IObtenerReseñasAprobadas _obtenerReseñasAprobadas;
    private readonly IDesaprobarReseña _desaprobarReseña;
    
    /// <summary>
    /// Inicializa una nueva instancia del <see cref="ReseñaController"/> con las dependencias necesarias.
    /// </summary>
    public ReseñaController(IAgregarReseña agregarReseña, IServicioImagenes servicioImagenes, IObtenerReseñasAprobadas obtenerReseñasAprobadas, IDesaprobarReseña desaprobarReseña)
    {
        _agregarReseña = agregarReseña;
        _servicioImagenes = servicioImagenes;
        _obtenerReseñasAprobadas = obtenerReseñasAprobadas;
        _desaprobarReseña = desaprobarReseña;
    }
    
    /// <summary>
    /// Crea una nueva reseña realizada por el usuario autenticado.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite que el usuario registrado envíe una reseña sobre un servicio recibido,
    /// pudiendo incluir opcionalmente una imagen asociada.
    ///
    /// **Flujo:**
    /// 1. Se valida el token JWT y se obtiene el identificador del usuario autenticado.  
    /// 2. Se procesa la información enviada en el formulario (<see cref="ReseñaACrearDto"/>).  
    /// 3. Si se incluye una imagen, se almacena mediante el servicio de imágenes <see cref="_servicioImagenes"/>  
    ///    y se obtiene una URL pública.  
    /// 4. Se ejecuta la lógica de creación mediante <see cref="_agregarReseña"/>.  
    /// 5. Se devuelve un objeto <see cref="ReseñaCreadaDto"/> con los datos de la reseña creada.
    ///
    /// **Requiere autenticación:**  
    /// - Solo disponible para usuarios con el rol <c>Cliente</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → La reseña fue creada correctamente.  
    /// - `400 Bad Request` → Faltan datos o alguno no es válido.  
    /// - `401 Unauthorized` → Token inválido o expirado.  
    /// - `403 Forbidden` → El usuario no tiene permisos.  
    /// - `409 Conflict` → La reseña no puede crearse debido a reglas de negocio.  
    /// - `500 Internal Server Error` → Error inesperado en el servidor.
    /// </remarks>
    /// <param name="nuevaReseña">
    /// Datos enviados por el usuario para crear la reseña, incluyendo descripción y calificación.
    /// </param>
    /// <param name="imagen">
    /// Archivo de imagen opcional asociado a la reseña.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación para interrumpir la operación si es necesario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP con los datos de la reseña creada.
    /// </returns>
    /// <response code="200">La reseña fue creada correctamente.</response>
    /// <response code="400">La solicitud contiene datos inválidos o la reseña fue catalogada como ofensiva.</response>
    /// <response code="401">Token inválido o expirado.</response>
    /// <response code="403">Permisos insuficientes para realizar la acción.</response>
    /// <response code="409">Conflicto según reglas de negocio.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    /// <response code="503">Error inesperado del servicio de OpenAI.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReseñaCreadaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Reseñar([FromForm] ReseñaACrearDto nuevaReseña, IFormFile? imagen, CancellationToken cancellationToken)
    {
        try
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim)) return Unauthorized(new { message = "Token inválido o expirado." });

            var idUsuario = int.Parse(idClaim);
            
            string? imagenUrl = null;

            if (imagen != null)
                imagenUrl = await _servicioImagenes.GuardarAsync(imagen, "resenias");
            
            var reseñaCreada = await _agregarReseña.Ejecutar(nuevaReseña, idUsuario, imagenUrl, cancellationToken);
            
            return Ok(reseñaCreada);
        } catch (ReseñaException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (ReseñaOfensivaException ex)
        {
            return BadRequest(new { message = ex.Message });
        } 
        catch (ModeracionIaNoDisponibleException )
        {
            return StatusCode(503, new { message = "No se pudo validar la reseña en este momento. Intentalo de nuevo más tarde." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }
    
    /// <summary>
    /// Obtiene el listado de reseñas aprobadas y visibles públicamente.
    /// </summary>
    /// <remarks>
    /// Este endpoint devuelve todas las reseñas que han sido previamente aprobadas
    /// y pueden ser mostradas en la sección pública de la aplicación.
    ///
    /// **Flujo:**
    /// 1. Consulta las reseñas aprobadas mediante el servicio <see cref="_obtenerReseñasAprobadas"/>.  
    /// 2. Devuelve una colección de objetos <see cref="ReseñaCreadaDto"/> con la información de cada reseña.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Lista de reseñas aprobadas obtenida correctamente.  
    /// - `500 Internal Server Error` → Error inesperado durante la obtención de las reseñas.
    /// </remarks>
    /// <param name="ct">
    /// Token de cancelación para interrumpir la operación si es necesario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP que contiene la colección de reseñas aprobadas.
    /// </returns>
    /// <response code="200">Lista de reseñas aprobadas obtenida correctamente.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReseñaCreadaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReseñasAprobadas(CancellationToken ct)
    {
        try
        {
            IEnumerable<ReseñaCreadaDto> reseñasAprobadas = await _obtenerReseñasAprobadas.Ejecutar(ct);
            return Ok(reseñasAprobadas);
        }
        catch (ReseñaException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado."});
        }
    }
    
    [Authorize(Roles = "Administrador")]
    [HttpPatch("{idReseña}/desaprobar")]
    public async Task<IActionResult> DesaprobarResena([FromRoute] int idReseña, CancellationToken ct)
    {
        try
        {
            await _desaprobarReseña.Ejecutar(idReseña, ct);
            return Ok(new { message = "Reseña desaprobada con éxito." });
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
}