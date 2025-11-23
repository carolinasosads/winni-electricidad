using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reservas;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class AgendarReserva : IAgendarReserva
{
    private readonly IRepositorioReserva _repositorioReserva;
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IRepositorioServicio _repositorioServicio;

    public AgendarReserva(IRepositorioReserva repositorioReserva, IRepositorioUsuario repositorioUsuario, IRepositorioServicio repositorioServicio)
    {
        _repositorioReserva = repositorioReserva;
        _repositorioUsuario = repositorioUsuario;
        _repositorioServicio = repositorioServicio;
    }
    
    public async Task<ReservaCreadaDto> Ejecutar(ReservaACrearDto nuevaReserva, int idUsuario, CancellationToken ct = default)
    {
        var usuario = await _repositorioUsuario.FindById(idUsuario, ct);
        if (usuario == null)
            throw new UnauthorizedAccessException("El usuario no existe o el token es inválido.");

        var direcciones = await _repositorioUsuario.FindAddressByUserId(idUsuario, ct);

        var direccion = direcciones.FirstOrDefault(d => d.IdDireccion == nuevaReserva.IdDireccion);

        if (direccion is null)
            throw new UnauthorizedAccessException("La dirección seleccionada no pertenece al usuario.");
        
        var servicios = await _repositorioServicio.FindByIds(nuevaReserva.IdServicios, ct);

        if (!servicios.Any())
            throw new ReservaException("Debes seleccionar al menos un servicio válido.");

        if (servicios.Any(s => !s.Activo))
            throw new ReservaException("Uno o más servicios seleccionados no están activos.");

        if (await _repositorioReserva.HorarioOcupado(nuevaReserva.FechaReserva, ct))
            throw new ReservaException("El horario seleccionado ya no está disponible.");

        if (await _repositorioReserva.UsuarioTieneReservaEnHorario(idUsuario, nuevaReserva.FechaReserva, ct))
            throw new ReservaException("Ya tienes una reserva en ese horario.");
        
        if (await _repositorioReserva.UsuarioTieneReservaEnDia(idUsuario, nuevaReserva.FechaReserva.Day, ct))
            throw new ReservaException("Ya tienes una reserva para este día. Si quieres agregar un nuevo servicio, simplemente comunícalo al representante que te contacte.");
        
        if (nuevaReserva.FechaReserva.DayOfWeek == DayOfWeek.Sunday)
            throw new ReservaException("No se puede reservar los domingos.");
        
        var reserva = ReservaMapper.MapearNuevaReservaDtoAEntidad(nuevaReserva, idUsuario, servicios);
        await _repositorioReserva.Add(reserva, ct);
        var reservaCompleta = await _repositorioReserva.FindById(reserva.IdReserva, ct);
        return reservaCompleta is not null ? ReservaMapper.MapearAReservaCreadaDto(reservaCompleta) : throw new ReservaException("Error inesperado.");
    }
}