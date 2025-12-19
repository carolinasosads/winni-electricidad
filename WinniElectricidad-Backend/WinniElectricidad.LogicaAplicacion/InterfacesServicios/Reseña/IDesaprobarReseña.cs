namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;

public interface IDesaprobarReseña
{
    Task Ejecutar(int idReserva, CancellationToken cancellationToken = default);
}