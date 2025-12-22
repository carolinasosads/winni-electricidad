namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;

public interface IDesactivarServicio
{
    Task Ejecutar(int idServicio, CancellationToken cancellationToken = default);
}