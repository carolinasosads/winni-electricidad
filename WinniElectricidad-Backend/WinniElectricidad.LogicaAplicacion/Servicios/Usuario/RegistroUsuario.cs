using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.Compartido.DTOs.Usuarios.Login;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Usuarios;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class RegistroUsuario : IRegistroUsuario
{
    private readonly IRepositorioUsuario _repositorioUsuario;

    public RegistroUsuario(IRepositorioUsuario repositorioUsuario)
    {
        _repositorioUsuario = repositorioUsuario;
    }
    
    public async Task<UsuarioLogueadoDto?> Registro(UsuarioRegistroDto usuarioRegistroDto, CancellationToken ct = default)
    {

        UsuarioCliente usuarioCliente = UsuarioMapper.MapearDtoRegistroAEntidad(usuarioRegistroDto);

        var existente = await _repositorioUsuario.FindbyEmail(usuarioRegistroDto.Email, ct);
        if (existente is not null)
        {
            throw new EmailEnUsoException("El email ya esta en uso.");
        }

        var usuarioRegistrado = await _repositorioUsuario.Registro(usuarioCliente, ct);
        
        if (usuarioRegistrado is not null)
        {
            UsuarioLogueadoDto usuarioLogueadoDto = UsuarioMapper.MappeoAUsuarioLogueadoDto(usuarioRegistrado);
            return usuarioLogueadoDto;
        }
        return null;
    }
    
}

