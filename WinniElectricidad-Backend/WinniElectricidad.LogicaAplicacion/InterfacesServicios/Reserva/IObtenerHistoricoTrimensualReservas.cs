using WinniElectricidad.Compartido.DTOs.Reservas;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IObtenerHistoricoTrimensualReservas
{
    Task<IEnumerable<HistoricoReservaDto>> Ejecutar(int mes, int anio, string? filtro, CancellationToken cancellationToken  = default);
}