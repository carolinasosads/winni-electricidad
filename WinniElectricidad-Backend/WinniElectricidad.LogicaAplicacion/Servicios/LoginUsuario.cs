using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios;

public class LoginUsuario : ILoginUsuario
{
    private readonly IRepositorioUsuario _repositorioUsuario;

    public LoginUsuario(IRepositorioUsuario repositorioUsuario)
    {
        _repositorioUsuario = repositorioUsuario;
    }
    public async Task<UsuarioLogueadoDto?> Login(string email, string password)
    {
        var usuarioLogueado = await _repositorioUsuario.Login(email, password);
        
        if (usuarioLogueado is not null)
        {
            UsuarioLogueadoDto usuarioLogueadoDto = UsuarioMapper.MappeoAUsuarioLogueadoDto(usuarioLogueado);
            return usuarioLogueadoDto;
        }

        return null;
    }
}