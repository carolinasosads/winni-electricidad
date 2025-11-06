using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios;

namespace WinniElectricidad.Api.Controllers;

/// <summary>
/// Controlador que gestiona las operaciones de autenticación de usuarios.
/// </summary>
[Route("WinniElectricidadApi/[controller]")]
[ApiController]
public class UsuarioController : ControllerBase
{
    private readonly ILoginUsuario _loginUsuario;
    private readonly IServicioToken _token;

    /// <summary>
    /// Inicializa una nueva instancia del <see cref="UsuarioController"/> con las dependencias necesarias.
    /// </summary>
    /// <param name="loginUsuario">Servicio de autenticación de usuarios.</param>
    /// <param name="token">Servicio para generación de tokens JWT.</param>
    public UsuarioController(ILoginUsuario loginUsuario, IServicioToken token)
    {
        _loginUsuario = loginUsuario;
        _token = token;
    }

    /// <summary>
    /// Autentica un usuario en el sistema utilizando sus credenciales de acceso.
    /// </summary>
    /// <remarks>
    /// Este endpoint valida el correo electrónico y la contraseña enviados por el cliente.
    /// Si las credenciales son válidas, genera un token JWT que permite al usuario acceder a las operaciones protegidas del sistema.
    /// 
    /// **Flujo:**
    /// 1. Verifica las credenciales con el servicio <see cref="ILoginUsuario"/>.
    /// 2. Si son correctas, genera un token mediante <see cref="IServicioToken"/>.
    /// 3. Devuelve un objeto <see cref="UsuarioLogueadoTokenDto"/> con los datos del usuario y el token.
    /// 
    /// **Códigos de respuesta:**
    /// - `200 OK` → Inicio de sesión exitoso. Devuelve el token JWT.
    /// - `401 Unauthorized` → Credenciales inválidas.
    /// - `500 Internal Server Error` → Error inesperado en el servidor.
    /// </remarks>
    /// <param name="usuarioLogin">Objeto que contiene el correo electrónico y la contraseña del usuario.</param>
    /// <returns>
    /// Una respuesta HTTP con el resultado de la autenticación.
    /// 
    /// Si es exitoso, el cuerpo incluye un <see cref="UsuarioLogueadoTokenDto"/> con el token de acceso.
    /// </returns>
    /// <response code="200">Inicio de sesión exitoso.</response>
    /// <response code="401">Credenciales inválidas o usuario no encontrado.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLogin)
    {
        try
        {
            UsuarioLogueadoDto? usuarioLogueadoDto = await _loginUsuario.Login(usuarioLogin.Email, usuarioLogin.Password);
            
            if (usuarioLogueadoDto is null)
            {
                return Unauthorized("Credenciales inválidas.");
            }
        
            var token = _token.GenerarToken(usuarioLogueadoDto.Id, usuarioLogueadoDto.Email, usuarioLogueadoDto.Rol);

            UsuarioLogueadoTokenDto usuarioLogueadoTokenDto = new UsuarioLogueadoTokenDto
            {
                Id = usuarioLogueadoDto.Id,
                Email = usuarioLogueadoDto.Email,
                Rol = usuarioLogueadoDto.Rol,
                Token = token
            };
        
            return  Ok(usuarioLogueadoTokenDto);
        } catch (Exception e)
        {
            return StatusCode(500, "Error inesperado.");
        }
    }
}