using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Registro;
using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Usuarios;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class CrearUsuarioDesdeAdmin : ICrearUsuarioDesdeAdmin
{
    private readonly IRepositorioUsuario _repositorioUsuario;

    public CrearUsuarioDesdeAdmin(IRepositorioUsuario repositorioUsuario)
    {
        _repositorioUsuario = repositorioUsuario;
    }

    public async Task<UsuarioCreadoDesdeAdminDto?> CrearUsuario(UsuarioRegistroAdminDto dto, CancellationToken ct = default)
    {
        var existente = await _repositorioUsuario.FindbyEmail(dto.Email, ct);
        if (existente is not null)
        {
            throw new EmailEnUsoException("El email ya está en uso.");
        }

        var passwordRandom = GenerarPasswordRandom();
        dto.Password = passwordRandom; 

        UsuarioCliente usuarioCliente = UsuarioMapper.MapearDtoRegistroAdminAEntidad(dto);

        var usuarioRegistrado = await _repositorioUsuario.Registro(usuarioCliente, ct);
        if (usuarioRegistrado is null) return null;

        
        return new UsuarioCreadoDesdeAdminDto 
        {
            Id = usuarioRegistrado.IdUsuario,
            Email = usuarioRegistrado.Email
        };
    }

    private string GenerarPasswordRandom()
    {
        return Guid.NewGuid().ToString("N")[..10]; 
    }
}