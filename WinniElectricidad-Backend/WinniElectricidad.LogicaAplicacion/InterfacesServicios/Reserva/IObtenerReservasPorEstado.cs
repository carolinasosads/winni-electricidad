using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IObtenerReservasPorEstado
{
    Task<IEnumerable<HistoricoReservaDto>> Ejecutar(string estado, string? filtro, CancellationToken ct = default);
}