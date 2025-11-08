using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.Compartido.DTOs.Usuarios.Login;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Usuarios;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios;

public class RegistroUsuario : IRegistroUsuario
{
    private readonly IRepositorioUsuario _repositorioUsuario;

    public RegistroUsuario(IRepositorioUsuario repositorioUsuario)
    {
        _repositorioUsuario = repositorioUsuario;
    }
    
    //Devuelve usuario logueado para poder tener el token
    public async Task<UsuarioLogueadoDto?> Registro(UsuarioRegistroDto usuarioRegistroDto)
    {

        UsuarioCliente usuarioCliente = UsuarioMapper.MapearDtoRegistroAEntidad(usuarioRegistroDto);

        var existente = await _repositorioUsuario.FindbyEmail(usuarioRegistroDto.Email);
        if (existente is not null)
        {
            throw new EmailEnUsoException("El email ya esta en uso.");
        }

        var usuarioRegistrado = await _repositorioUsuario.Registro(usuarioCliente);
        
        if (usuarioRegistrado is not null)
        {
            //Devuelve usaurio logueado por el mismo motivo
            UsuarioLogueadoDto usuarioLogueadoDto = UsuarioMapper.MappeoAUsuarioLogueadoDto(usuarioRegistrado);
            return usuarioLogueadoDto;
        }
        return null;
    }
    
}

