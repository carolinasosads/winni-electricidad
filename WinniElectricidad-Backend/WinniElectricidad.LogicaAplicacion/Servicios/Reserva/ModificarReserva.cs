using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class ModificarReserva : IModificarReserva
{
    private readonly IRepositorioReserva _repositorioReserva;

    public ModificarReserva(IRepositorioReserva repositorioReserva)
    {
        _repositorioReserva = repositorioReserva;
    }

    public async Task Ejecutar(ReservaAModificarDto dto, CancellationToken cancellationToken = default)
    {
        var reserva = await _repositorioReserva.ObtenerReservaPorId(dto.IdReserva, cancellationToken);

        if (reserva is null)
            throw new ArgumentException("La reserva no existe.");

        var desde = dto.NuevaFecha.AddMinutes(-5);
        var hasta = dto.NuevaFecha.AddMinutes(5);

        var reservasEnRango = await _repositorioReserva
            .FindAllBetweenDates(desde, hasta, cancellationToken);
        reserva.Reprogramar(dto.NuevaFecha, reservasEnRango);

        await _repositorioReserva.ActualizarReserva(reserva, cancellationToken);
    }
}
