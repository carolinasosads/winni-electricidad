using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class AprobarReserva : IAprobarReserva
{
    private readonly IEnviarEmail _enviarEmail;
    private readonly IRepositorioReserva _repositorioReserva;
    private readonly IRepositorioUsuario _repositorioUsuario;


    public AprobarReserva(IRepositorioReserva repoReserva, IRepositorioUsuario repositorioUsuario,
        IEnviarEmail enviarEmail)
    {
        _repositorioReserva = repoReserva;
        _enviarEmail = enviarEmail;
        _repositorioUsuario = repositorioUsuario;
    }
    
        public async Task Ejecutar(int idReserva, int idUsuario, bool esAdmin, CancellationToken cancellationToken = default)
    {
        var reserva = await _repositorioReserva.ObtenerReservaPorId(idReserva, cancellationToken);
        if (reserva is null)
            throw new ArgumentException("La reserva no existe.");

        if (esAdmin)
        {
            if (reserva.EstadoReserva != EstadoReserva.Pendiente)
                throw new InvalidOperationException("Solo se pueden aprobar reservas pendientes.");

            reserva.RequiereConfirmacionCliente = false;
            reserva.EstadoReserva = EstadoReserva.Confirmada;
        }
        else
        {
            if (reserva.IdUsuarioCliente != idUsuario)
                throw new InvalidOperationException("No tenés permisos para aprobar esta reserva.");

            reserva.Aprobar();
        }

        await _repositorioReserva.Update(reserva, cancellationToken);

        var cliente = reserva.UsuarioCliente;
        var fecha = reserva.FechaReserva.ToString("dd/MM/yyyy HH:mm");

        var cuerpoEmailCliente = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Reserva confirmada</h2>
                <p>Hola {cliente.NombreCompleto},</p>
                <p>Tu reserva para el <strong>{fecha}</strong> fue <strong>confirmada</strong>.</p>
                <p>¡Gracias por confiar en Winni Electricidad!</p>
            </div>";

        await _enviarEmail.Ejecutar(cliente.Email, "Winni Electricidad - Reserva confirmada", cuerpoEmailCliente, cancellationToken);

        var admin = await _repositorioUsuario.ObtenerAdministrador(cancellationToken);

        var cuerpoEmailAdmin = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Reserva confirmada</h2>
                <p><strong>Cliente:</strong> {cliente.NombreCompleto}</p>
                <p><strong>Email:</strong> {cliente.Email}</p>
                <p><strong>Fecha de la reserva:</strong> {fecha}</p>
                <p>La reserva fue marcada como <strong>confirmada</strong> en el sistema.</p>
            </div>";

        await _enviarEmail.Ejecutar(admin.Email, "Reserva confirmada – Notificación interna", cuerpoEmailAdmin, cancellationToken);
    }
}