using WinniElectricidad.Compartido.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class RegistrarReservaHistoricaAdmin:IRegistrarReservaHistoricaAdmin
{
    private readonly IRepositorioUsuario _repoUsuario;
    private readonly IRepositorioServicio _repoServicio;
    private readonly IRepositorioReserva _repoReserva;
    
    public RegistrarReservaHistoricaAdmin(IRepositorioUsuario repoUsuario, IRepositorioServicio repoServicio, IRepositorioReserva repoReserva)
    {
        _repoUsuario = repoUsuario;
        _repoServicio = repoServicio;
        _repoReserva = repoReserva;
    }

    public async Task<ReservaCreadaDto> Ejecutar(ReservaACrearDto dto, int idUsuarioCliente, CancellationToken ct = default)
    {
        var usuario = await _repoUsuario.FindById(idUsuarioCliente, ct);
        if (usuario is null) throw new ArgumentException("El cliente no existe.");

        var direcciones = await _repoUsuario.FindAddressByUserId(idUsuarioCliente, ct);
        var direccion = direcciones.FirstOrDefault(d => d.IdDireccion == dto.IdDireccion);
        if (direccion is null) throw new ArgumentException("La dirección no pertenece al cliente.");

        var servicios = await _repoServicio.FindByIds(dto.IdServicios, ct);
        if (!servicios.Any()) throw new ArgumentException("Debe seleccionar al menos un servicio válido.");

        var reserva = WinniElectricidad.LogicaNegocio.Entidades.Reserva.CrearHistoricaAdmin(
            dto.FechaReserva,
            dto.TipoServicio,
            idUsuarioCliente,
            dto.IdDireccion,
            servicios.ToList(),
            dto.Comentario
        );

        await _repoReserva.Add(reserva, ct);

        var completa = await _repoReserva.FindById(reserva.IdReserva, ct);
        if (completa is null) throw new Exception("Error inesperado al recuperar la reserva.");

        return WinniElectricidad.Compartido.DTOs.Mappers.ReservaMapper.MapearAReservaCreadaDto(completa);
    }
}