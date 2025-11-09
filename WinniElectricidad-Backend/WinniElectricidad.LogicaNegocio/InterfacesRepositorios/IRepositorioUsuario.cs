using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorioUsuario : IRepositorio<UsuarioBase>
{
    Task<UsuarioBase?> FindbyEmail(string email, CancellationToken ct = default);
    Task<UsuarioBase?> Login (string email, string password, CancellationToken ct = default);
    Task<UsuarioCliente?> Registro(UsuarioCliente usuarioCliente, CancellationToken ct = default);
    Task ChangePassword(int idUsuario, string password, CancellationToken ct = default);
}