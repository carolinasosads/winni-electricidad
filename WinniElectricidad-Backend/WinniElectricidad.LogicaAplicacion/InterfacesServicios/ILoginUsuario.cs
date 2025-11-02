using WinniElectricidad.Compartido.DTOs.Usuarios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios;

public interface ILoginUsuario
{
    Task<UsuarioLogueadoDto?> Login(string email, string password);
}