using WinniElectricidad.Compartido.DTOs.Direcciones;
using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.Compartido.DTOs.Usuarios.Login;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.DTOs.Mappers;

public class UsuarioMapper
{
    public static UsuarioLogueadoDto MappeoAUsuarioLogueadoDto(UsuarioBase usuario)
    {
        return new UsuarioLogueadoDto
        {
            Id = usuario.IdUsuario,
            Email = usuario.Email,
            Rol = usuario.Rol,
        };
    }

    public static UsuarioCliente MapearDtoRegistroAEntidad(UsuarioRegistroDto usuarioRegistroDto, string passwordHash)
    {
        var direcciones = (usuarioRegistroDto.Direcciones ?? new List<DireccionDto>())
            .Where(d => !string.IsNullOrWhiteSpace(d.Calle)
                        && !string.IsNullOrWhiteSpace(d.Esquina))
            .Select(d => new Direccion
            {
                Calle   = d.Calle!.Trim(),
                Esquina = d.Esquina!.Trim(),
                Numero  = string.IsNullOrWhiteSpace(d.Numero) ? null : d.Numero!.Trim(),
                Apto    = string.IsNullOrWhiteSpace(d.Apto)   ? null : d.Apto!.Trim()
            })
            .ToList();

        return new UsuarioCliente(
            usuarioRegistroDto.NombreCompleto.Trim(),
            passwordHash,
            usuarioRegistroDto.Email.Trim().ToLowerInvariant(),
            usuarioRegistroDto.Telefono,
            direcciones
        );
    }
}