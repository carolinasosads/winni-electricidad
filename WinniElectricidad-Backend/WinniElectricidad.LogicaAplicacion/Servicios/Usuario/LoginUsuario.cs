using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Usuarios.Login;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class LoginUsuario : ILoginUsuario
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IServicioHash _servicioHash;

    public LoginUsuario(IRepositorioUsuario repositorioUsuario, IServicioHash servicioHash)
    {
        _repositorioUsuario = repositorioUsuario;
        _servicioHash = servicioHash;
    }
    public async Task<UsuarioLogueadoDto?> Login(string email, string password, CancellationToken ct = default)
    {
        if(email is null || password is null){ return null; }

        var usuarioParaLoguear = await _repositorioUsuario.FindbyEmail(email, ct);
        
        if (usuarioParaLoguear == null){ return null; }

        var passwordHash = usuarioParaLoguear.PasswordHash;
        var credencialesCorrectas = _servicioHash.VerificarPassword(password, passwordHash);
        
        if (!credencialesCorrectas) return null;
        
        UsuarioLogueadoDto usuarioLogueadoDto = UsuarioMapper.MappeoAUsuarioLogueadoDto(usuarioParaLoguear);
        
        return usuarioLogueadoDto;
    }
}