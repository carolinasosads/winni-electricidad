using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Presupuesto;

public class ObtenerPresupuesto: IObtenerPresupuesto
{
    private readonly IRepositorioPresupuesto _repoPresupuesto;

    public ObtenerPresupuesto(IRepositorioPresupuesto repoPresupuesto)
    {
        _repoPresupuesto = repoPresupuesto;
    }

    public async Task<PresupuestoDto> Ejecutar(int idReserva, CancellationToken ct = default)
    {
        if (idReserva <= 0)
            throw new ArgumentException("El id de la reserva es inválido.");

        var presupuesto = await _repoPresupuesto.FindByReservaId(idReserva, ct);

        if (presupuesto is null)
            throw new KeyNotFoundException("No existe presupuesto para la reserva indicada.");

        return PresupuestoMapper.MapearAPresupuestoDto(presupuesto);
    }
}