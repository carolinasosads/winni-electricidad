using WinniElectricidad.Compartido.DTOs.Usuarios.ListadoUsuarios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface IObtenerUsuariosPorServicio
{
    Task<IEnumerable<ListadoUsuariosDto>> Ejecutar(int idServicio, CancellationToken ct);
}