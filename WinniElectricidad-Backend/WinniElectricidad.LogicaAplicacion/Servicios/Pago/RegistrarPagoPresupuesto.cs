using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Pago;
using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Pago;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Pago;

public class RegistrarPagoPresupuesto : IRegistrarPagoPresupuesto
{
    private readonly IRepositorioPresupuesto _repoPresupuesto;
    private readonly IRepositorioPago _repoPago;

    public RegistrarPagoPresupuesto(IRepositorioPresupuesto repoPresupuesto, IRepositorioPago repoPago)
    {
        _repoPresupuesto = repoPresupuesto;
        _repoPago = repoPago;
    }

    public async Task<PresupuestoDto> Registrar(int idReserva, PagoCrearDto dto, CancellationToken ct = default)
    {
        if (idReserva <= 0)
            throw new ArgumentException("El id de la reserva es inválido.");

        if (dto is null)
            throw new ArgumentNullException(nameof(dto), "El body es requerido.");

        if (dto.Monto <= 0)
            throw new ArgumentException("El monto del pago debe ser mayor a 0.");

        var presupuesto = await _repoPresupuesto.FindByReservaId(idReserva, ct);
        if (presupuesto is null)
            throw new KeyNotFoundException("No existe presupuesto para esa reserva.");

        var pagadoActual = presupuesto.Pagos?.Sum(p => p.Monto) ?? 0m;
        var nuevoTotalPagado = pagadoActual + dto.Monto;

        if (nuevoTotalPagado > presupuesto.Monto)
            throw new ArgumentException("El pago supera el monto total del presupuesto.");

        var pago = PagoMapper.MapearAPago(dto, presupuesto.Id, presupuesto.IdUsuario);
        await _repoPago.Add(pago, ct);

        presupuesto.MontoPagado = nuevoTotalPagado;
        await _repoPresupuesto.Update(presupuesto, ct);

        var presupuestoActualizado = await _repoPresupuesto.FindByReservaId(idReserva, ct);
        if (presupuestoActualizado is null)
            throw new KeyNotFoundException("No existe presupuesto para esa reserva.");

        return PresupuestoMapper.MapearAPresupuestoDto(presupuestoActualizado);
    }
}