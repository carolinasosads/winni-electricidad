using WinniElectricidad.Compartido.DTOs.Direcciones;
using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.Compartido.DTOs.Registro;
using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.Compartido.DTOs.Usuarios.ListadoUsuarios;
using WinniElectricidad.Compartido.DTOs.Usuarios.Login;
using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;
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
    
    public static UsuarioCliente MapearDtoRegistroAdminAEntidad(UsuarioRegistroAdminDto dto, string pass)
    {
        return new UsuarioCliente
        {
            NombreCompleto = dto.NombreCompleto,
            Email = dto.Email,
            Telefono = dto.Telefono,
            PasswordHash = pass,
            Direcciones = dto.Direcciones?
                .Where(d => !string.IsNullOrWhiteSpace(d.Calle)
                            && !string.IsNullOrWhiteSpace(d.Esquina))
                .Select(d => new Direccion
                {
                    Calle   = d.Calle!.Trim(),
                    Esquina = d.Esquina!.Trim(),
                    Numero  = string.IsNullOrWhiteSpace(d.Numero) ? null : d.Numero!.Trim(),
                    Apto    = string.IsNullOrWhiteSpace(d.Apto)   ? null : d.Apto!.Trim()
                })
                .ToList()
        };
    }
    
    public static ListadoUsuariosDto MapearAListadoUsuariosDto(UsuarioBase u)
    {
        return new ListadoUsuariosDto
        {
            IdUsuario = u.IdUsuario,
            Email = u.Email,
            NombreCompleto = u.NombreCompleto,
            Telefono = u.Telefono
        };
    }

    public static IReadOnlyList<ListadoUsuariosDto> MapearAListadoUsuariosDto(IEnumerable<UsuarioBase> usuarios)
    {
        return usuarios.Select(MapearAListadoUsuariosDto).ToList();
    }


public static DetalleUsuariosDto MapearADetalleUsuariosDto(UsuarioCliente usuario)
{
    return new DetalleUsuariosDto
    {
        Email = usuario.Email,
        NombreCompleto = usuario.NombreCompleto,
        Telefono = usuario.Telefono,

        Direcciones = (usuario.Direcciones ?? new List<Direccion>())
            .Select(d => new DireccionDto
            {
                Calle = d.Calle,
                Esquina = d.Esquina,
                Numero = d.Numero,
                Apto = d.Apto
            })
            .ToList(),

        Presupuestos = (usuario.Presupuestos ?? new List<LogicaNegocio.Entidades.Presupuesto>())
            .Select(p => new PresupuestoDto
            {
                Id = p.Id,
                IdReserva = p.IdReserva,
                MontoTotal = p.Monto,
                FechaCreacion = p.FechaPresupuesto 
            })
            .ToList(),

        Reservas = new List<UsuarioReservaDto>()
    };
}

public static UsuarioReservaDto MapearAUsuarioReservaDto(UsuarioBase usuario)
{
    return new UsuarioReservaDto
    {
        IdUsuario = usuario.IdUsuario,
        Nombre = usuario.NombreCompleto,
        Email = usuario.Email,
        Telefono = usuario.Telefono
    };
}

public static IReadOnlyList<UsuarioReservaDto> MapearAUsuarioReservaDto(IEnumerable<UsuarioBase> usuarios)
{
    return usuarios.Select(MapearAUsuarioReservaDto).ToList();
}
}