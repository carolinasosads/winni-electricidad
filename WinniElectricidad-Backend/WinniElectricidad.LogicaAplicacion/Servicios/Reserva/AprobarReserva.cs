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
        var footer = _enviarEmail.GetFooter();

        var cuerpoEmailCliente = $@"
          <div style=""font-family: Arial, sans-serif; background-color:#f4f6f8; padding:24px;"">
            <div style=""max-width:600px; margin:0 auto; background-color:#ffffff; border-radius:8px; overflow:hidden;"">

              <div style=""background-color:#1f3a5f; color:#ffffff; padding:16px 24px;"">
                <h2 style=""margin:0; font-size:20px;"">Winni Electricidad</h2>
              </div>

              <div style=""padding:24px; color:#333333;"">
                <h3 style=""margin-top:0; color:#1f3a5f;"">Reserva confirmada</h3>

                <p style=""margin:0 0 12px 0;"">
                  Hola <strong>{cliente.NombreCompleto}</strong> 👋🏽,
                </p>

                <p style=""margin:0 0 16px 0;"">
                  Tu reserva para el <strong>{fecha}</strong> fue <strong>confirmada</strong>.
                </p>

                <p style=""margin:0;"">
                  Gracias por confiar en <strong>Winni Electricidad</strong>.
                </p>

                {footer}
              </div>

            </div>
          </div>";

        await _enviarEmail.Ejecutar(cliente.Email, "Winni Electricidad - Reserva confirmada", cuerpoEmailCliente, cancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        var admin = await _repositorioUsuario.ObtenerAdministrador(cancellationToken);

        var cuerpoEmailAdmin = $@"
          <div style=""font-family: Arial, sans-serif; background-color:#f4f6f8; padding:24px;"">
            <div style=""max-width:600px; margin:0 auto; background-color:#ffffff; border-radius:8px; overflow:hidden;"">

              <div style=""background-color:#1f3a5f; color:#ffffff; padding:16px 24px;"">
                <h2 style=""margin:0; font-size:20px;"">Winni Electricidad</h2>
              </div>

              <div style=""padding:24px; color:#333333;"">
                <h3 style=""margin-top:0; color:#1f3a5f;"">Reserva confirmada</h3>

                <p style=""margin:0 0 8px 0;"">
                  <strong>Cliente:</strong> {cliente.NombreCompleto}
                </p>
                <p style=""margin:0 0 8px 0;"">
                  <strong>Email:</strong> {cliente.Email}
                </p>
                <p style=""margin:0 0 16px 0;"">
                  <strong>Fecha de la reserva:</strong> {fecha}
                </p>

                <p style=""margin:0;"">
                  La reserva fue marcada como <strong>confirmada</strong> en el sistema.
                </p>

                {footer}
              </div>

            </div>
          </div>";

        await _enviarEmail.Ejecutar(admin.Email, "Reserva confirmada – Notificación interna", cuerpoEmailAdmin, cancellationToken);
    }
}