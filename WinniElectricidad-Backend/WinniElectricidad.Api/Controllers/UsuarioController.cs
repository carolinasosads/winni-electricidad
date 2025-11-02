using Microsoft.AspNetCore.Mvc;
using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios;

namespace WinniElectricidad.Api.Controllers;

[Route("WinniElectricidadApi/[controller]")]
[ApiController]
public class UsuarioController : ControllerBase
{
    private readonly ILoginUsuario _loginUsuario;
    private readonly IServicioToken _token;

    public UsuarioController(ILoginUsuario loginUsuario, IServicioToken token)
    {
        _loginUsuario = loginUsuario;
        _token = token;
    }

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
            return StatusCode(500, "Error inesperado.");
        }
    }
}