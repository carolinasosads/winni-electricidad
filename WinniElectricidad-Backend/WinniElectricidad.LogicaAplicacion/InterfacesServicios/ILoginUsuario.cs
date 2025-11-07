using WinniElectricidad.Compartido.DTOs.Usuarios;
using WinniElectricidad.Compartido.DTOs.Usuarios.Login;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios;

public interface ILoginUsuario
{
    Task<UsuarioLogueadoDto?> Login(string email, string password, CancellationToken ct);
}