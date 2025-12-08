using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;
using WinniElectricidad.Compartido.Reservas;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.DTOs.Mappers;

public class ReservaMapper
{
    public static Reserva MapearNuevaReservaDtoAEntidad(ReservaACrearDto nuevaReserva, int idUsuario, IReadOnlyList<Servicio> servicios)
    {
        var comentario = string.IsNullOrWhiteSpace(nuevaReserva.Comentario)
            ? null
            : nuevaReserva.Comentario.Trim();
        
        return new Reserva(
            fechaReserva: nuevaReserva.FechaReserva,
            tipo: nuevaReserva.TipoServicio,
            idUsuarioCliente: idUsuario,
            idDireccion: nuevaReserva.IdDireccion,
            servicios: servicios.ToList(),
            comentario: comentario
        );
    }

    public static ReservaCreadaDto MapearAReservaCreadaDto(Reserva reserva)
    {
        return new ReservaCreadaDto
        {
            IdReserva = reserva.IdReserva,
            FechaReserva = reserva.FechaReserva,
            TipoServicio = reserva.TipoServicioReserva.ToString(),
            Direccion = $"{reserva.Direccion.Calle} {reserva.Direccion.Numero ?? ""} - Esquina {reserva.Direccion.Esquina}",
            Servicios = reserva.Servicios.Select(s => s.Titulo).ToList(),
            Comentario = reserva.Comentario
        };
    }
    
    public static HistoricoReservaDto MapearAHistoricoReservaDto(Reserva reserva)
    {
        return new HistoricoReservaDto
        {
            IdReserva = reserva.IdReserva,
            FechaReserva = reserva.FechaReserva,
            Estado = reserva.EstadoReserva.ToString(),
            TipoServicio = reserva.TipoServicioReserva.ToString(),
            Comentario = reserva.Comentario,

            Cliente = new UsuarioReservaDto
            {
                IdUsuario = reserva.UsuarioCliente.IdUsuario,
                Nombre = reserva.UsuarioCliente.NombreCompleto,
                Email = reserva.UsuarioCliente.Email,
                Telefono = reserva.UsuarioCliente.Telefono
            },

            Direccion = DireccionMapper.MapearDireccionADto(reserva.Direccion),
            Servicios = ServicioMapper.MapearServiciosADtos(reserva.Servicios),
                
        };
    }

}