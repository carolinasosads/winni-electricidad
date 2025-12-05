using WinniElectricidad.Compartido.DTOs.Reservas;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IObtenerHistoricoMensualReservas
{
    Task<IEnumerable<HistoricoReservaDto>> Ejecutar(int mes, int anio, CancellationToken cancellationToken  = default);
}