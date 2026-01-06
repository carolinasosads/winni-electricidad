using WinniElectricidad.Compartido.DTOs.Pago;
using WinniElectricidad.Compartido.DTOs.Presupuesto;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Pago;

public interface IRegistrarPagoPresupuesto
{
    Task<PresupuestoDto> Registrar(int idReserva, PagoCrearDto dto, CancellationToken ct = default);
}