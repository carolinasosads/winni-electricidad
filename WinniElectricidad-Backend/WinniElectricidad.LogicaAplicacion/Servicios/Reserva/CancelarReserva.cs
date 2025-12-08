using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class CancelarReserva : ICancelarReserva
{
    private readonly IRepositorioReserva _repositorioReserva;
    private readonly IEnviarEmail _enviarEmail;
    private readonly IRepositorioUsuario _repositorioUsuario;


    public CancelarReserva(IRepositorioReserva repoReserva, IEnviarEmail enviarEmail, IRepositorioUsuario repositorioUsuario)
    {
        _repositorioReserva = repoReserva;
        _enviarEmail = enviarEmail;
        _repositorioUsuario = repositorioUsuario;
    }
    public async Task Ejecutar(int idReserva, CancellationToken cancellationToken = default)
    {
        var reserva = await _repositorioReserva.ObtenerReservaPorId(idReserva, cancellationToken);

        if (reserva is null)
            throw new ArgumentException("La reserva no existe.");

        reserva.Cancelar();
        await _repositorioReserva.ActualizarReserva(reserva, cancellationToken);

        var cliente = reserva.UsuarioCliente 
                      ?? throw new InvalidOperationException("El cliente asociado a la reserva no existe.");

        var fecha = reserva.FechaReserva.ToString("dd/MM/yyyy HH:mm");
        var direccion = reserva.Direccion?.ToString() ?? "Sin dirección registrada";
        var comentario = string.IsNullOrWhiteSpace(reserva.Comentario)
            ? "Sin comentarios adicionales." : reserva.Comentario;
        var tipoServicio = reserva.TipoServicioReserva.ToString();

        var cuerpoCliente = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Reserva cancelada</h2>
                <p>Hola {cliente.NombreCompleto},</p>
                <p>Te informamos que tu reserva para el <strong>{fecha}</strong> fue <strong>cancelada</strong>.</p>
                <p>Si crees que se trata de un error o querés reprogramar, podés agendar una nueva reserva desde la plataforma.</p>
                <p>¡Gracias por confiar en Winni Electricidad!</p>
            </div>";

        await _enviarEmail.Ejecutar(cliente.Email, "Winni Electricidad - Reserva cancelada", cuerpoCliente, cancellationToken);

 
        var admin = await _repositorioUsuario.ObtenerAdministrador(cancellationToken);
        
        var cuerpoAdmin = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Reserva cancelada</h2>

                <p><strong>Cliente:</strong> {cliente.NombreCompleto}</p>
                <p><strong>Email:</strong> {cliente.Email}</p>
                <p><strong>Teléfono:</strong> {cliente.Telefono}</p>

                <p><strong>Tipo de servicio:</strong> {tipoServicio}</p>
                <p><strong>Dirección:</strong> {direccion}</p>

                <p><strong>Fecha de la reserva cancelada:</strong> {fecha}</p>

                <p><strong>Comentario del cliente:</strong> {comentario}</p>

                <p>Esta reserva ha sido marcada como <strong>cancelada</strong> en el sistema.</p>
            </div>";

        await _enviarEmail.Ejecutar(admin.Email, "Reserva cancelada – Notificación interna", cuerpoAdmin, cancellationToken);
     }
}