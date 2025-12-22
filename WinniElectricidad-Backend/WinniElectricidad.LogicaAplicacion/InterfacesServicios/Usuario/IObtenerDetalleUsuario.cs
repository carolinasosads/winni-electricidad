using WinniElectricidad.Compartido.DTOs.Usuarios.ListadoUsuarios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface IObtenerDetalleUsuario
{
    Task<DetalleUsuariosDto?> Execute(int idUsuario, CancellationToken ct = default);
}