using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Presupuesto;

public class ObtenerPresupuestoPorReserva : IObtenerPresupuestoPorReserva
{
    private readonly IRepositorioPresupuesto _repositorioPresupuesto;

public ObtenerPresupuestoPorReserva(IRepositorioPresupuesto repo)
    {
        _repositorioPresupuesto = repo;
    }

    public async Task<PresupuestoDto?> Ejecutar(int idReserva, CancellationToken ct = default)
    {
        if (idReserva <= 0) throw new ArgumentException("Id de reserva inválido.");

        var presupuesto = await _repositorioPresupuesto.FindByReservaId(idReserva, ct);
        if (presupuesto is null) return null;

        return PresupuestoMapper.MapearAPresupuestoDto(presupuesto);
    }
}