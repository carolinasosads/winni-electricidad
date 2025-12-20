using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Presupuesto;

public class ActualizarMontoPagadoPresupuesto: IActualizarMontoPagadoPresupuesto
{
    private readonly IRepositorioPresupuesto _repositorioPresupuesto;

    public ActualizarMontoPagadoPresupuesto(IRepositorioPresupuesto repoPresupuesto)
    {
        _repositorioPresupuesto = repoPresupuesto;
    }

    public async Task<PresupuestoDto> Actualizar(int idReserva, PresupuestoMontoPagadoActualizarDto dto, CancellationToken ct = default)
    {
        {
            if (idReserva <= 0)
                throw new ArgumentException("El id de la reserva es inválido.");

            var presupuesto = await _repositorioPresupuesto.FindByReservaId(idReserva, ct);
            if (presupuesto is null)
                throw new Exception("No existe presupuesto para esa reserva.");
            
            var montoTotal = presupuesto.Monto;

            if (dto.MontoPagado < 0) throw new ArgumentException("El monto pagado no puede ser negativo.");

            if (dto.MontoPagado > montoTotal) throw new ArgumentException("El monto pagado no puede ser mayor al monto total del presupuesto.");

            presupuesto.MontoPagado = dto.MontoPagado;

            await _repositorioPresupuesto.Update(presupuesto, ct);

            return PresupuestoMapper.MapearAPresupuestoDto(presupuesto);
        }
    }
}