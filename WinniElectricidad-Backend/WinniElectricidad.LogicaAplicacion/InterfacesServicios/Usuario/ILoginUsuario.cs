using WinniElectricidad.Compartido.DTOs.Usuarios.Login;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface ILoginUsuario
{
    Task<UsuarioLogueadoDto?> Login(string email, string password, CancellationToken ct = default);
}