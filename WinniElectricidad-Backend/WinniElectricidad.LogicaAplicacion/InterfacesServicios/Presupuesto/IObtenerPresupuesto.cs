using WinniElectricidad.Compartido.DTOs.Presupuesto;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;

public interface IObtenerPresupuesto
{
    Task<PresupuestoDto> Ejecutar(int idReserva, CancellationToken ct = default);
}