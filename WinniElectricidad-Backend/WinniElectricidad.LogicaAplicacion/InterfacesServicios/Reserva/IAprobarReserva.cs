namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IAprobarReserva
{
    Task Ejecutar(int idReserva, int idUsuario, bool esAdmin, CancellationToken cancellationToken = default);
}