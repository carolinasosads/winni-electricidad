namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;

public interface IActivarServicio
{
    Task Ejecutar(int idServicio, CancellationToken cancellationToken = default);
}