using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorioUsuario : IRepositorio<UsuarioBase>
{
    Task<UsuarioBase?> FindbyEmail(string email, CancellationToken ct = default);
    Task<UsuarioCliente?> Registro(UsuarioCliente usuarioCliente, CancellationToken ct = default);
    Task ChangePassword(int idUsuario, string password, CancellationToken ct = default);
    Task<IReadOnlyList<Direccion>> FindAddressByUserId (int idUsuario, CancellationToken ct = default);
    Task<UsuarioAdministrador?> ObtenerAdministrador(CancellationToken ct = default);
    Task<ICollection<UsuarioCliente>> BuscarPorNombreEmailTelefono (string dato, CancellationToken ct = default);
    Task<IReadOnlyList<UsuarioBase>> FindByIds(List<int> ids, CancellationToken ct = default);
}