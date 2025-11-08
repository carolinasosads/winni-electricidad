using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.Compartido.DTOs.Usuarios.Login;
using WinniElectricidad.Compartido.DTOs.Usuarios.RecuperacionContrasena;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Usuarios;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Notificaciones;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Tokens;

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
    
    private readonly IRecuperarContrasena _recuperarContrasena;


    /// <summary>
    /// Inicializa una nueva instancia del <see cref="UsuarioController"/> con las dependencias necesarias.
    /// </summary>
    /// <param name="loginUsuario">Servicio de autenticación de usuarios.</param>
    /// <param name="token">Servicio para generación de tokens JWT.</param>
    /// <param name="recuperarContrasena">Servicio de recuperación de contraseña.</param>
    /// <param name="registroUsuario">Servicio para registrar usuarios.</param>
    public UsuarioController(ILoginUsuario loginUsuario, IServicioToken token, IRegistroUsuario registroUsuario, IHCaptchaVerifier captcha, IRecuperarContrasena recuperarContrasena)
    {
        _loginUsuario = loginUsuario;
        _token = token;
        _registroUsuario = registroUsuario;
        _captcha = captcha;
        _recuperarContrasena = recuperarContrasena;
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
    /// <param name="ct">Token de cancelación para abortar la operación si es necesario.</param>
    /// <returns>
    /// Una respuesta HTTP con el resultado de la autenticación.
    /// 
    /// Si es exitoso, el cuerpo incluye un <see cref="UsuarioLogueadoTokenDto"/> con el token de acceso.
    /// </returns>
    /// <response code="200">Inicio de sesión exitoso.</response>
    /// <response code="401">Credenciales inválidas o usuario no encontrado.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(UsuarioLogueadoTokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLogin, CancellationToken ct)
    {
        try
        {
            UsuarioLogueadoDto? usuarioLogueadoDto = await _loginUsuario.Login(usuarioLogin.Email, usuarioLogin.Password, ct);
            
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
        } catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado."});
        }
    }

    /// <summary>
    /// Inicia el proceso de recuperación de contraseña para un usuario registrado.
    /// </summary>
    /// <remarks>
    /// Este endpoint envía un correo electrónico con las instrucciones para restablecer la contraseña.
    ///
    /// **Flujo:**
    /// 1. Verifica si el correo pertenece a un usuario registrado mediante <see cref="IRecuperarContrasena"/>.
    /// 2. Si existe, genera un enlace temporal de recuperación y lo envía por correo.
    /// 3. Por motivos de seguridad, la respuesta siempre es la misma.
    ///
    /// **Códigos de respuesta:**
    /// - `200 OK` → Solicitud procesada (si el correo existe, se envían las instrucciones).
    /// - `500 Internal Server Error` → Error al enviar el correo o error inesperado.
    /// </remarks>
    /// <param name="usuarioForgotPassword">
    /// Objeto que contiene el correo electrónico del usuario que solicita la recuperación.
    /// </param>
    /// <param name="ct">Token de cancelación para abortar la operación si es necesario.</param>
    /// 
    /// <returns>
    /// Una respuesta HTTP que indica el resultado del proceso de recuperación.
    /// </returns>
    /// <response code="200">Solicitud procesada correctamente.</response>
    /// <response code="500">Error al enviar el correo o error inesperado del servidor.</response>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ForgotPassword([FromBody] UsuarioForgotPasswordDto usuarioForgotPassword, CancellationToken ct)
    {
        try
        {
            await _recuperarContrasena.EnviarCorreoRecuperacion(usuarioForgotPassword.Email, ct);
            return Ok(new { message = "Si el correo existe, te enviamos un enlace para restablecer tu contraseña." });
        } 
        catch (EmailNotificacionException)
        {
            return StatusCode(500, new { message = "Error al enviar el correo. Intenta nuevamente más tarde."});
        } 
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado."} );
        }
    }

    /// <summary>
    /// Restablece la contraseña de un usuario a partir de un token válido de recuperación.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite al usuario establecer una nueva contraseña, siempre que el token de recuperación sea válido y no haya expirado.
    /// 
    /// **Flujo:**
    /// 1. El usuario accede al enlace recibido por correo, el cual contiene un token de un solo uso.  
    /// 2. Envía el token y la nueva contraseña al servidor.  
    /// 3. El sistema valida el token mediante <see cref="IRecuperarContrasena"/>.  
    /// 4. Si el token es válido, se actualiza la contraseña del usuario y se invalida el token.
    /// 
    /// **Códigos de respuesta:**
    /// - `200 OK` → La contraseña fue restablecida exitosamente.  
    /// - `400 Bad Request` → El token es inválido o ya expiró.  
    /// - `500 Internal Server Error` → Error inesperado en el servidor.
    /// </remarks>
    /// <param name="usuarioResetPassword">
    /// Objeto que contiene la nueva contraseña y el token de recuperación.
    /// </param>
    /// <param name="ct">Token de cancelación para abortar la operación si es necesario.</param>
    /// <returns>
    /// Una respuesta HTTP que indica el resultado del proceso de restablecimiento.
    /// </returns>
    /// <response code="200">Contraseña restablecida correctamente.</response>
    /// <response code="400">El token ya expiró o es inválido.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResetPassword([FromBody] UsuarioResetPasswordDto usuarioResetPassword,
        CancellationToken ct)
    {
        try
        {
            await _recuperarContrasena.ResetearContrasena(usuarioResetPassword.Password, usuarioResetPassword.TokenPlain,ct);
            return Ok( new { message = "La contraseña fue modificada con éxito."});
        } 
        catch (OneTimeTokenException)
        {
            return BadRequest(new { message = "El link ya expiró."});
        } 
        catch (Exception)
        {
            return StatusCode(500, new { message = "Error inesperado."});
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
            return Conflict(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new{message = "Error inesperado." });
        }
    }
}