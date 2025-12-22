using WinniElectricidad.Compartido.DTOs.Presupuesto;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;

public interface IObtenerPresupuestoPorReserva
{
    Task<PresupuestoDto?> Ejecutar(int idReserva, CancellationToken ct = default);
}