using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.Compartido.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reservas;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class AgendarReserva : IAgendarReserva
{
    private readonly IRepositorioReserva _repositorioReserva;
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IRepositorioServicio _repositorioServicio;
    private readonly IEnviarEmail _enviarEmail;

    public AgendarReserva(IRepositorioReserva repositorioReserva, IRepositorioUsuario repositorioUsuario, IRepositorioServicio repositorioServicio, IEnviarEmail enviarEmail)
    {
        _repositorioReserva = repositorioReserva;
        _repositorioUsuario = repositorioUsuario;
        _repositorioServicio = repositorioServicio;
        _enviarEmail = enviarEmail;
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
        
        if (await _repositorioReserva.UsuarioTieneReservaEnDia(idUsuario, nuevaReserva.FechaReserva, ct))
            throw new ReservaException("Ya tienes una reserva para este día. Si quieres agregar un nuevo servicio, simplemente comunícalo al representante que te contacte.");
        
        var reserva = ReservaMapper.MapearNuevaReservaDtoAEntidad(nuevaReserva, idUsuario, servicios);
        await _repositorioReserva.Add(reserva, ct);
        
        var reservaCompleta = await _repositorioReserva.FindById(reserva.IdReserva, ct);
        if (reservaCompleta is null){
            throw new ReservaException("Error inesperado.");
        }
        
        var dto = ReservaMapper.MapearAReservaCreadaDto(reservaCompleta);

        await EnviarCorreosReserva(dto, usuario, ct);

        return dto;
    }
    
    private async Task EnviarCorreosReserva(
        ReservaCreadaDto reserva, 
        UsuarioBase usuario, 
        CancellationToken ct)
    {
        var nombreCliente = usuario.NombreCompleto;
        var emailCliente = usuario.Email;
        var fecha = reserva.FechaReserva.ToString("dd/MM/yyyy HH:mm");
        var direccion = reserva.Direccion;
        var comentario = string.IsNullOrWhiteSpace(reserva.Comentario)
            ? "Sin comentarios adicionales."
            : reserva.Comentario;
        var serviciosTexto = string.Join(", ", reserva.Servicios);
        var numeroTelefono = usuario.Telefono;

        var admin = await _repositorioUsuario.ObtenerAdministrador(ct);

        if (admin is null) throw new ReservaException("No hay un usuario administrador.");
        
        var cuerpoEmailCliente = $@"
              <div style='font-family: Arial, sans-serif; color: #333;'>
                   <h2>Reserva recibida (pendiente de confirmación)</h2>

                   <p>Hola {nombreCliente},</p>

                   <p>Recibimos tu solicitud de reserva para un presupuesto. 
                   <strong>La reserva está pendiente de revisión.</strong> </p>

                   <p>A continuación compartimos los datos que registró:</p>

                   <p><strong>Fecha solicitada:</strong> {fecha}</p>

                   <p><strong>Servicios seleccionados:</strong></p>
                   <p>{serviciosTexto}</p>

                   <p><strong>Comentario:</strong> {comentario}</p>

                   <p>Un miembro del equipo de Winni Electricidad se va a contactar contigo en breve y 
                   <strong>coordinarán la confirmación de la reserva</strong>.</p>

                   <p>¡Gracias por confiar en Winni Electricidad!</p>
               </div>";

        var cuerpoEmailAdmin = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h2>Nueva reserva de presupuesto agendada</h2>
                    <p><strong>Nombre del cliente:</strong> {nombreCliente}</p>
                    <p><strong>Email:</strong> {emailCliente}</p>
                    <p><strong>Teléfono:</strong> {numeroTelefono}</p>
                    <p><strong>Fecha:</strong> {fecha}</p>
                    <p><strong>Dirección:</strong> {direccion}</p>
                    <p><strong>Tipo de servicio:</strong> {reserva.TipoServicio}</p>
                    <p><strong>Servicios:</strong></p>
                    <p>{serviciosTexto}</p>
                    <p><strong>Comentario del cliente:</strong> {comentario}</p>
                    <p><strong>¡No olvides confirmarla o sugerir una modificación de fecha en tu panel de reservas!</strong></p>
                </div>";

        await _enviarEmail.Ejecutar(emailCliente, "Winni Electricidad - Reserva de presupuesto", cuerpoEmailCliente, ct);
        await _enviarEmail.Ejecutar(admin.Email, "Nueva reserva de presupuesto agendada", cuerpoEmailAdmin, ct);
    }
}