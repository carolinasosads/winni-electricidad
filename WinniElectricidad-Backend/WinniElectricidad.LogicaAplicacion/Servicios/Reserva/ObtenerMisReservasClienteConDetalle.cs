using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;
using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.CasosDeUso.Reservas;

public class ObtenerMisReservasClienteConDetalle : IObtenerMisReservasClienteConDetalle
{
    private readonly IRepositorioReserva _repoReserva;

    public ObtenerMisReservasClienteConDetalle(IRepositorioReserva repoReserva)
    {
        _repoReserva = repoReserva;
    }

    public async Task<IReadOnlyList<ReservaListadoDto>> Ejecutar(int idCliente, CancellationToken ct)
    {
        var reservas = await _repoReserva.ObtenerReservasClienteConDetalle(idCliente, ct);

        return reservas.Select(ReservaMapper.MapearAReservaListadoDto).ToList();
    }
}