using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class ObtenerReservasPorEstado : IObtenerReservasPorEstado
{
    private readonly IRepositorioReserva _repositorioReserva;

    public ObtenerReservasPorEstado(IRepositorioReserva repositorioReserva)
    {
        _repositorioReserva = repositorioReserva;
    }

    public async Task<IEnumerable<HistoricoReservaDto>> Ejecutar(string estado, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(estado))
            throw new ArgumentException("Debe indicar un estado.");

        // Mapear string a enum 
        if (!Enum.TryParse<EstadoReserva>(estado, ignoreCase: true, out var estadoEnum))
            throw new ArgumentException($"El estado '{estado}' no es válido.", nameof(estado));

        var reservas = await _repositorioReserva.FindAllSegunEstado(estadoEnum, ct);

        return reservas
            .Select(ReservaMapper.MapearAHistoricoReservaDto)
            .ToList();
    }
}