using WinniElectricidad.Compartido.DTOs.Usuarios;
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
}