namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface ICancelarReserva
{
    Task Ejecutar(int idReserva, CancellationToken cancellationToken = default);
}