namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IAprobarReserva
{
    Task Ejecutar(int idReserva, CancellationToken cancellationToken = default);
}