using WinniElectricidad.Compartido.DTOs.Presupuesto;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;

public interface IActualizarMontoPagadoPresupuesto
{
    Task<PresupuestoDto> Actualizar(int idReserva, PresupuestoMontoPagadoActualizarDto dto, CancellationToken ct = default);
}