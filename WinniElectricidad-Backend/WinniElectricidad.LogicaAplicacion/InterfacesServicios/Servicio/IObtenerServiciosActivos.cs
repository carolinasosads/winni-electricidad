using WinniElectricidad.Compartido.DTOs.Servicios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;

public interface IObtenerServiciosActivos
{
    Task<IEnumerable<ServicioActivoDto>> Ejecutar(CancellationToken ct = default);
}