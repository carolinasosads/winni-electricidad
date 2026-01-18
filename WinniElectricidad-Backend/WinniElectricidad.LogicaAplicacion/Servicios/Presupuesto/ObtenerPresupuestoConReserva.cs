using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Presupuesto;

public class ObtenerPresupuestoConReserva : IObtenerPresupuestoConReserva
{
    
    private readonly IRepositorioPresupuesto _repositorioPresupuesto;

    public ObtenerPresupuestoConReserva(IRepositorioPresupuesto repo)
    {
        _repositorioPresupuesto = repo;
    }
    
    public async Task<IEnumerable<PresupuestoConReservaDto>> Ejecutar(int idUsuario, CancellationToken ct = default)
    {
        if (idUsuario <= 0) throw new ArgumentException("Id de reserva inválido.");

        var presupuestos = await _repositorioPresupuesto.ObtenerPresupuestosConReservaPorUsuario(idUsuario, ct);
        if (presupuestos is null) return null;

        return presupuestos
            .Select(PresupuestoMapper.MapearAPresupuestoConReservaDto)
            .ToList();    
    }
}