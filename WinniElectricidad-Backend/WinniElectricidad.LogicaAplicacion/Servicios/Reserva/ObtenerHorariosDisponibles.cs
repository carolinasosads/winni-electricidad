using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class ObtenerHorariosDisponibles : IObtenerHorariosDisponibles
{
    private readonly IRepositorioReserva _repositorioReserva;

    public ObtenerHorariosDisponibles(IRepositorioReserva repositorioReserva)
    {
        _repositorioReserva = repositorioReserva;
    }
    public async Task<IEnumerable<DiaDisponibilidadDto>> Ejecutar(CancellationToken ct = default)
    {
        var hoy = DateTime.Today;
        var minimo = hoy.AddDays(2);
        var maximo = hoy.AddDays(30);
        var reservas = await _repositorioReserva.FindAllBetweenDates(minimo, maximo, ct);
        
        var horariosDisponibles = new List<DiaDisponibilidadDto>();
        
        for (var fecha = minimo; fecha <= maximo; fecha = fecha.AddDays(1))
        {
            var horas = new List<HoraDto>();

            var horaActual = new TimeOnly(9, 0);
            var fin = new TimeOnly(17, 0);

            while (horaActual < fin)
            {
                bool ocupado = reservas.Any(r =>
                    r.FechaReserva.Date == fecha.Date &&
                    TimeOnly.FromDateTime(r.FechaReserva) == horaActual);

                horas.Add(new HoraDto { Hora = horaActual, Disponible = !ocupado });
                horaActual = horaActual.AddMinutes(90); 
            }
            
            horariosDisponibles.Add(new DiaDisponibilidadDto { Fecha = fecha, Horas = horas });
        }

        return horariosDisponibles;
    }
}