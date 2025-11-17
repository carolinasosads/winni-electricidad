using WinniElectricidad.Compartido.DTOs.Reservas;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IObtenerHorariosDisponibles
{
    Task<IEnumerable<DiaDisponibilidadDto>> Ejecutar(CancellationToken ct = default);
}