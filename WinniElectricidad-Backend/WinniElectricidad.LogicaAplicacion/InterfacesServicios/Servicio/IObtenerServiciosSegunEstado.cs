using WinniElectricidad.Compartido.DTOs.Servicios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;

public interface IObtenerServiciosSegunEstado
{
    Task<IEnumerable<ServicioActivoDto>> Ejecutar(bool activo, CancellationToken ct = default);
}