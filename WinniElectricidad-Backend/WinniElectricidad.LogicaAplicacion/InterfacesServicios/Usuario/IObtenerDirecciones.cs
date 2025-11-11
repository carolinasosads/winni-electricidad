using WinniElectricidad.Compartido.DTOs.Direcciones;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface IObtenerDirecciones
{
    Task<IEnumerable<DireccionDetalleDto>> Ejecutar(int idUsuario, CancellationToken ct = default);
}