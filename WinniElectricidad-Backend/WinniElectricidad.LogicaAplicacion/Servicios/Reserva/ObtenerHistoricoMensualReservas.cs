using WinniElectricidad.Compartido.DTOs.Direcciones;
using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class ObtenerHistoricoMensualReservas : IObtenerHistoricoMensualReservas
{
    private readonly IRepositorioReserva _repositorioReserva;

    public ObtenerHistoricoMensualReservas(IRepositorioReserva repositorioReserva)
    {
        _repositorioReserva = repositorioReserva;
    }

    public async Task<IEnumerable<HistoricoReservaDto>> Ejecutar(int mes, int anio,  CancellationToken cancellationToken = default)
    {
        if (mes < 1 || mes > 12)
            throw new ArgumentException("El mes debe estar entre 1 y 12.");
        
        if (anio < 2000 || anio > DateTime.Today.Year + 1)
            throw new ArgumentException("El año es inválido.");
        
        var inicioMes = new DateTime(anio, mes, 1);
        var finMes = inicioMes.AddMonths(1).AddTicks(-1);

        var reservas = await _repositorioReserva.FindAllBetweenDates(inicioMes, finMes, cancellationToken);

        var resultado = reservas
            .Select(r => ReservaMapper.MapearAHistoricoReservaDto(r))
            .ToList();

        return resultado;
    }
}