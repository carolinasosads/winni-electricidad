using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class ObtenerHorariosDisponibles : IObtenerHorariosDisponibles
{
    private readonly IRepositorioReserva _repositorioReserva;
    private readonly IRepositorioSettings _repositorioSettings;

    public ObtenerHorariosDisponibles(IRepositorioReserva repositorioReserva, IRepositorioSettings repositorioSettings)
    {
        _repositorioReserva = repositorioReserva;
        _repositorioSettings = repositorioSettings;
    }
    public async Task<IEnumerable<DiaDisponibilidadDto>> Ejecutar(CancellationToken ct = default)
    {
        var config =  await _repositorioSettings.Obtener(ct);
        var hoy = DateTime.Today;
        var minimo = hoy.AddDays(config.DiasMinimos);
        var maximo = hoy.AddDays(config.DiasMaximos);
        var reservas = await _repositorioReserva.FindAllBetweenDates(minimo, maximo, ct);
        
        var horariosDisponibles = new List<DiaDisponibilidadDto>();
        
        for (var fecha = minimo; fecha <= maximo; fecha = fecha.AddDays(1))
        {
            var horas = new List<HoraDto>();
            
            if (fecha.DayOfWeek != DayOfWeek.Sunday)
            {
                var horaActual = config.HoraInicio;
                var fin = config.HoraFin;
                
                while (horaActual < fin)
                {
                    bool ocupado = reservas.Any(r =>
                        r.FechaReserva.Date == fecha.Date &&
                        TimeOnly.FromDateTime(r.FechaReserva) == horaActual);

                    horas.Add(new HoraDto { Hora = horaActual, Disponible = !ocupado });
                    
                    horaActual = horaActual.AddMinutes(config.MinutosEntreTurnos); 
                }
                
                horariosDisponibles.Add(new DiaDisponibilidadDto { Fecha = fecha, Horas = horas });
            }
        }

        return horariosDisponibles;
    }
}