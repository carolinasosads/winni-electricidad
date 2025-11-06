using WinniElectricidad.Compartido.DTOs.Usuarios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios;

public interface IRegistroUsuario
{
        Task<UsuarioLogueadoDto?> Registro(UsuarioRegistroDto usuarioRegistroDto);
}