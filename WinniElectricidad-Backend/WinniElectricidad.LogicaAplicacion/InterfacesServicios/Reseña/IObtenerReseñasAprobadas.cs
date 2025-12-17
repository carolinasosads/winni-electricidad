using WinniElectricidad.Compartido.DTOs.Reseñas;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;

public interface IObtenerReseñasAprobadas
{
    Task<IEnumerable<ReseñaCreadaDto>> Ejecutar(CancellationToken ct = default);
}