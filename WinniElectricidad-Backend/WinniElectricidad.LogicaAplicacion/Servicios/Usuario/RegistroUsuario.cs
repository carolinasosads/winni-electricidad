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
    private readonly IServicioHash _servicioHash;

    public RegistroUsuario(IRepositorioUsuario repositorioUsuario,  IServicioHash servicioHash)
    {
        _repositorioUsuario = repositorioUsuario;
        _servicioHash = servicioHash;
    }
    
    public async Task<UsuarioLogueadoDto?> Registro(UsuarioRegistroDto usuarioRegistroDto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(usuarioRegistroDto.Password))
            throw new ArgumentException("La contraseña es obligatoria.");

        if (usuarioRegistroDto.Password.Length < 6)
            throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");
        
        var existente = await _repositorioUsuario.FindbyEmail(usuarioRegistroDto.Email, ct);
        if (existente is not null)
        {
            throw new EmailEnUsoException("El email ya esta en uso.");
        }
        
        var passwordHash = _servicioHash.Hash(usuarioRegistroDto.Password);
        
        UsuarioCliente usuarioCliente = UsuarioMapper.MapearDtoRegistroAEntidad(usuarioRegistroDto, passwordHash);
        
        var usuarioRegistrado = await _repositorioUsuario.Registro(usuarioCliente, ct);
        
        if (usuarioRegistrado is not null)
        {
            UsuarioLogueadoDto usuarioLogueadoDto = UsuarioMapper.MappeoAUsuarioLogueadoDto(usuarioRegistrado);
            return usuarioLogueadoDto;
        }
        
        return null;
    }
    
}

