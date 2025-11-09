using WinniElectricidad.Compartido.DTOs.Servicios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;

public interface IObtenerServiciosDisponibles
{
    Task<IEnumerable<ServicioDisponibleDto>> Ejecutar(CancellationToken ct = default);
}