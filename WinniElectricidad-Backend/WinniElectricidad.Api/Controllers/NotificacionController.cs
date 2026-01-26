using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Compartido.DTOs.Recordatorios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;

namespace WinniElectricidad.Api.Controllers;

/// <summary>
/// Controlador que gestiona las operaciones de notificación enviadas por el admin manualmente.
/// </summary>
[Route("WinniElectricidadApi/[controller]")]
[ApiController]
public class NotificacionController : ControllerBase
{
    private readonly IEnviarRecordatorioEstacional _enviarRecordatorioEstacional;

    /// <summary>
    /// Inicializa una nueva instancia del <see cref="NotificacionController"/> con las dependencias necesarias.
    /// </summary>
    /// <param name="enviarRecordatorioEstacional">Servicio para enviar recordatorios estacionales.</param>
    public NotificacionController(IEnviarRecordatorioEstacional enviarRecordatorioEstacional)
    {
        _enviarRecordatorioEstacional = enviarRecordatorioEstacional;
    }
    
    /// <summary>
    /// Envía recordatorios estacionales a un conjunto de clientes seleccionados.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite al administrador enviar comunicaciones por correo electrónico
    /// relacionadas con un servicio específico, con el objetivo de recordar mantenimientos
    /// o acciones recomendadas según la temporada.
    ///
    /// **Flujo:**
    /// 1. Recibe la información del recordatorio mediante <see cref="RecordatorioDto"/>.  
    /// 2. Ejecuta el envío de correos utilizando el servicio <see cref="_enviarRecordatorioEstacional"/>.  
    /// 3. Devuelve una confirmación si el proceso finaliza correctamente.
    ///
    /// La operación puede involucrar el envío de múltiples correos electrónicos.
    ///
    /// **Requiere autenticación:**  
    /// - Solo disponible para usuarios con el rol <c>Administrador</c>.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Recordatorios enviados correctamente.  
    /// - `500 Internal Server Error` → Error inesperado durante el envío.
    /// </remarks>
    /// <param name="recordatorio">
    /// Información del recordatorio a enviar, incluyendo el servicio, el texto del mensaje
    /// y los correos electrónicos de los clientes destinatarios.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación para abortar el proceso de envío si es necesario.
    /// </param>
    /// <returns>
    /// Una respuesta HTTP que confirma el envío de los recordatorios.
    /// </returns>
    /// <response code="200">Recordatorios enviados correctamente.</response>
    /// <response code="400">La solicitud contiene datos inválidos.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpPost("enviar-recordatorio")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> EnviarRecordatorio([FromBody] RecordatorioDto recordatorio, CancellationToken cancellationToken)
    {
        try
        {
            await _enviarRecordatorioEstacional.Ejecutar(recordatorio.TituloServicio, recordatorio.Texto, recordatorio.EmailClientesParaEnviar, cancellationToken);
            
            return Ok("¡Recordatorios enviados con éxito!");
        } 
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado." });
        }
    }
}