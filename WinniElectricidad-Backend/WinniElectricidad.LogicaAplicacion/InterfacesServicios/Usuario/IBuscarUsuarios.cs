using WinniElectricidad.Compartido.DTOs.Usuarios.Busqueda;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface IBuscarUsuarios
{
    Task<ICollection<UsuarioBusquedaDto>> BuscarUsuariosAsync(string query, CancellationToken ct);
}