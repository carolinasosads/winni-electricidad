using WinniElectricidad.Compartido.DTOs.Servicios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;

public interface IEditarServicio
{
    Task<ServicioActivoDto> Ejecutar(int idServicio, EditarServicioDto servicio, ICollection<string>? imagenesUrl, CancellationToken ct = default);
}