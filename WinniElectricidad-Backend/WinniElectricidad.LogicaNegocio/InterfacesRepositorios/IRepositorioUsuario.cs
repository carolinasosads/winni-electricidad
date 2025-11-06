using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorioUsuario : IRepositorio<UsuarioBase>
{
    public Task<UsuarioBase?> FindbyEmail(string email);
    public Task<UsuarioBase?> Login (string email, string password);   
    Task<UsuarioCliente?> Registro(UsuarioCliente usuarioCliente);
}