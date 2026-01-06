using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Compartido.DTOs.Consulta;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Consulta;

namespace WinniElectricidad.Api.Controllers;

[ApiController]
[Route("WinniElectricidadApi/[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly ICrearConsulta _crearConsulta;

    public ConsultasController(ICrearConsulta crearConsulta)
    {
        _crearConsulta = crearConsulta;
    }

    /// <summary>
    /// Crea una consulta general y la envía al administrador.
    /// </summary>
    /// <remarks>
    /// Este endpoint está disponible para usuarios logueados y no logueados.
    /// Valida el cuerpo recibido según las reglas de <see cref="ConsultaCrearDto"/> y, si es válido,
    /// delega el procesamiento en <see cref="ICrearConsulta"/>.
    /// </remarks>
    /// <param name="dto">Datos de la consulta a crear.</param>
    /// <param name="ct">Token de cancelación para abortar la operación.</param>
    /// <returns>
    /// 200 OK si la consulta se procesó correctamente; 400 BadRequest si el modelo es inválido.
    /// </returns>
    /// <response code="200">La consulta fue enviada correctamente.</response>
    /// <response code="400">El body es inválido o faltan campos requeridos.</response>
    [HttpPost("crear")]
    [AllowAnonymous]
    public async Task<IActionResult> Crear([FromBody] ConsultaCrearDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _crearConsulta.Ejecutar(dto, ct);

        return Ok(new { message = "Consulta enviada correctamente." });
    }
}