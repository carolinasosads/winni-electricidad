using WinniElectricidad.Compartido.DTOs.Servicios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;

public interface ICrearServicio
{
    Task<ServicioDto> Ejecutar(CrearServicioDto nuevoServicio, ICollection<string> imagenesUrl, CancellationToken ct = default);
}