using WinniElectricidad.Compartido.DTOs.Presupuesto;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;

public interface IObtenerPresupuestoConReserva
{
    Task<IEnumerable<PresupuestoConReservaDto>> Ejecutar(int idUsuario, CancellationToken ct = default);
}