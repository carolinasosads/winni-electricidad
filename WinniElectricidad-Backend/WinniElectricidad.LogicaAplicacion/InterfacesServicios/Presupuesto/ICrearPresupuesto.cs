using WinniElectricidad.Compartido.DTOs.Presupuesto;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;

public interface ICrearPresupuesto
{
    Task<PresupuestoDto> Ejecutar(int idReserva, PresupuestoCrearDto dto, CancellationToken ct = default);
}