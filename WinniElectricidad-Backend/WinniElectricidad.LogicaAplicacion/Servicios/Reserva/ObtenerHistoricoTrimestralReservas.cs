using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class ObtenerHistoricoTrimestralReservas : IObtenerHistoricoTrimensualReservas
{
    private readonly IRepositorioReserva _repositorioReserva;

    public ObtenerHistoricoTrimestralReservas(IRepositorioReserva repositorioReserva)
    {
        _repositorioReserva = repositorioReserva;
    }

    public async Task<IEnumerable<HistoricoReservaDto>> Ejecutar(int mes, int anio, string? filtro, CancellationToken cancellationToken = default)
    {
        if (mes < 1 || mes > 12)
            throw new ArgumentException("El mes debe estar entre 1 y 12.");
        
        if (anio < 2000 || anio > DateTime.Today.Year + 2)
            throw new ArgumentException("El año es inválido.");
        
        var primerDiaMesActual = new DateTime(anio, mes, 1);

        var inicio = primerDiaMesActual.AddMonths(-1);
        var finExclusive = primerDiaMesActual.AddMonths(2);

        var reservas = await _repositorioReserva.FindHistoricoReservas(inicio, finExclusive, filtro, cancellationToken);
        
        var resultado = reservas
            .Select(r => ReservaMapper.MapearAHistoricoReservaDto(r))
            .ToList();

        return resultado;
    }
}