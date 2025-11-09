using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.Compartido.DTOs.Usuarios.Login;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface IRegistroUsuario
{
        Task<UsuarioLogueadoDto?> Registro(UsuarioRegistroDto usuarioRegistroDto, CancellationToken ct = default);
}