using WinniElectricidad.Compartido.DTOs.Direcciones;
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
            Servicios = ServicioMapper.MapearServiciosADtos(true, reserva.Servicios),
                
        };
    }

    
    public static ReservaListadoDto MapearAReservaListadoDto(Reserva reserva)
    {
        var servicios = (reserva.Servicios ?? new List<Servicio>())
            .Select(s => s.Titulo)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList();

        var nombreServicio = servicios.FirstOrDefault() ?? string.Empty;

        var direccionDescripcion = reserva.Direccion != null
            ? $"{reserva.Direccion.Calle} {reserva.Direccion.Numero ?? ""}".Trim()
            : string.Empty;

        return new ReservaListadoDto
        {
            IdReserva = reserva.IdReserva,
            FechaReserva = reserva.FechaReserva,
            Estado = reserva.EstadoReserva.ToString(),
            NombreServicio = nombreServicio,
            Servicios = servicios,
            TienePresupuesto = reserva.Presupuesto != null,
            MontoPresupuestado = reserva.Presupuesto?.Monto ?? 0m,

            RequiereConfirmacionCliente = reserva.RequiereConfirmacionCliente,

            Direccion = reserva.Direccion == null
                ? null
                : new WinniElectricidad.Compartido.DTOs.Direcciones.DireccionDto
                {
                    IdDireccion = reserva.Direccion.IdDireccion,
                    Calle = reserva.Direccion.Calle,
                    Esquina = reserva.Direccion.Esquina,
                    Numero = reserva.Direccion.Numero,
                    Apto = reserva.Direccion.Apto
                }
        };
    }
}