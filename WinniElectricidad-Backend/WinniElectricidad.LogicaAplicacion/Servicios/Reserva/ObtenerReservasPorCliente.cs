using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class ObtenerReservasPorCliente : IObtenerReservasPorCliente
{
    private readonly IRepositorioReserva _repoReserva;

    public ObtenerReservasPorCliente(IRepositorioReserva repoReserva)
    {
        _repoReserva = repoReserva;
    }

    public async Task<IEnumerable<ReservaListadoDto>> EjecutarAsync(int clienteId, CancellationToken ct = default)
    {
        var reservas = await _repoReserva.GetReservaSegunClienteId(clienteId, ct);

        return reservas
            .Select(ReservaMapper.MapearAReservaListadoDto)
            .ToList();
    }
}