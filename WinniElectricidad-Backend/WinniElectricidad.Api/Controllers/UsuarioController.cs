using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Usuarios;

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
    private readonly IRegistroUsuario _registroUsuario; 
    private readonly IHCaptchaVerifier _captcha;
    /// <summary>
    /// Inicializa una nueva instancia del <see cref="UsuarioController"/> con las dependencias necesarias.
    /// </summary>
    /// <param name="loginUsuario">Servicio de autenticación de usuarios.</param>
    /// <param name="token">Servicio para generación de tokens JWT.</param>
    public UsuarioController(ILoginUsuario loginUsuario, IServicioToken token, IRegistroUsuario registroUsuario, IHCaptchaVerifier captcha)
    {
        _loginUsuario = loginUsuario;
        _token = token;
        _registroUsuario = registroUsuario;
        _captcha = captcha;
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
                return Unauthorized();
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
            return StatusCode(500, new{message = "Error inesperado." });
        }
    }
    
     /// <summary>
    /// Registra un nuevo usuario (valida hCaptcha) y devuelve token JWT + rol.
    /// </summary>
    /// <response code="201">Usuario creado. Devuelve token y rol.</response>
    /// <response code="400">Datos inválidos o captcha no verificado.</response>
    /// <response code="409">El email ya está en uso.</response>
    /// <response code="500">Error inesperado.</response>
    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto)
    {
        try
        {
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var captchaOk = await _captcha.VerifyAsync(usuarioRegistroDto.HCaptchaToken, remoteIp);
            if (!captchaOk)
            {
                return BadRequest(new { message = "Captcha no verificado." });
            }

            var creado = await _registroUsuario.Registro(usuarioRegistroDto);
            if (creado is null)
            {
                return StatusCode(500, new{message = "No se pudo crear el usuario."});

            }

            // genera token igual que en login
            var jwt = _token.GenerarToken(creado.Id, creado.Email, creado.Rol);

            var usuarioLogueadoTokenDto = new UsuarioLogueadoTokenDto
            {
                Id = creado.Id,
                Email = creado.Email,
                Rol = creado.Rol,
                Token = jwt
            };
            return Ok(usuarioLogueadoTokenDto);
        }
        catch (EmailEnUsoException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new{message = "Error inesperado." });
        }
    }
}