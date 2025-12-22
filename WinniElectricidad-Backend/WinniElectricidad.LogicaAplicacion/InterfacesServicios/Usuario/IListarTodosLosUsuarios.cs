using WinniElectricidad.Compartido.DTOs.Usuarios.ListadoUsuarios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface IListarTodosLosUsuarios
{
    Task<IReadOnlyList<ListadoUsuariosDto>> Listar(CancellationToken ct = default);
}