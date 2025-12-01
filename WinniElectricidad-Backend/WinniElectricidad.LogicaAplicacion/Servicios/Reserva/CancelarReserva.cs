using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class CancelarReserva : ICancelarReserva
{
    private readonly IRepositorioReserva _repositorioReserva;

    public CancelarReserva(IRepositorioReserva repoReserva)
    {
        _repositorioReserva = repoReserva;
    }
    public async Task Ejecutar(int idReserva, CancellationToken cancellationToken = default)
    {
        var reserva = await _repositorioReserva.ObtenerReservaPorId(idReserva, cancellationToken);

        if (reserva is null)
            throw new InvalidOperationException("La reserva no existe.");

        reserva.Cancelar();
        
        reserva.EstadoReserva = EstadoReserva.Cancelada;

        await _repositorioReserva.ActualizarReserva(reserva, cancellationToken);
    }
}