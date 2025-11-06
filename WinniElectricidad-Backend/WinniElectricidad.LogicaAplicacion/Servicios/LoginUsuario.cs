using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Usuarios.Login;
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
    public async Task<UsuarioLogueadoDto?> Login(string email, string password, CancellationToken ct = default)
    {
        if(email is null || password is null){ return null; }
        
        var usuarioLogueado = await _repositorioUsuario.Login(email, password, ct);

        if (usuarioLogueado is null) return null;
        
        UsuarioLogueadoDto usuarioLogueadoDto = UsuarioMapper.MappeoAUsuarioLogueadoDto(usuarioLogueado);
        
        return usuarioLogueadoDto;

    }
}