using WinniElectricidad.Compartido.DTOs.Servicios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;

public interface ICrearServicio
{
    Task<ServicioDto> Ejecutar(ServicioDto nuevoServicio, CancellationToken ct = default);
}