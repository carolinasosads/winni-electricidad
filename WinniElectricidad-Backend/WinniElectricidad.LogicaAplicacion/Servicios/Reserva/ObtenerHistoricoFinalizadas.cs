using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class ObtenerHistoricoFinalizadas : IObtenerHistoricoFinalizadas
{
    private readonly IRepositorioReserva _repositorioReserva;

    public ObtenerHistoricoFinalizadas(IRepositorioReserva repo)
    {
        _repositorioReserva = repo;
    }

    public async Task<IEnumerable<HistoricoReservaDto>> Ejecutar(string? filtro, CancellationToken ct = default)
    {
        var reservas = await _repositorioReserva.FindAllFinalizadas(filtro, ct);

        return reservas
            .Select(reserva => ReservaMapper.MapearAHistoricoReservaDto(reserva))
            .ToList();
    }
}