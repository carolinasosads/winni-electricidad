using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
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
        await _repositorioReserva.Update(reserva, cancellationToken);

        var cliente = reserva.UsuarioCliente 
                      ?? throw new InvalidOperationException("El cliente asociado a la reserva no existe.");

        var fecha = reserva.FechaReserva.ToString("dd/MM/yyyy HH:mm");
        var direccion = $"{reserva.Direccion.Calle}"
                        + (string.IsNullOrWhiteSpace(reserva.Direccion.Apto)
                          ? ""
                          : $" Apto {reserva.Direccion.Apto}")
                        + (string.IsNullOrWhiteSpace(reserva.Direccion.Esquina)
                          ? ""
                          : $" Esq. {reserva.Direccion.Esquina}");
        var comentario = string.IsNullOrWhiteSpace(reserva.Comentario)
            ? "Sin comentarios adicionales." : reserva.Comentario;
        var tipoServicio = reserva.TipoServicioReserva.ToString();

        var footer = _enviarEmail.GetFooter();

        var cuerpoCliente = $@"
            <div style=""font-family: Arial, sans-serif; background-color:#f4f6f8; padding:24px;"">
              <div style=""max-width:600px; margin:0 auto; background-color:#ffffff; border-radius:8px; overflow:hidden;"">

                <!-- Header -->
                <div style=""background-color:#1f3a5f; color:#ffffff; padding:16px 24px;"">
                  <h2 style=""margin:0; font-size:20px;"">Winni Electricidad</h2>
                </div>

                <!-- Body -->
                <div style=""padding:24px; color:#333333;"">
                  <h3 style=""margin-top:0; color:#1f3a5f;"">Reserva cancelada</h3>

                  <p style=""margin:0 0 12px 0;"">
                    Hola <strong>{cliente.NombreCompleto} 👋🏽</strong>,
                  </p>

                  <p style=""margin:0 0 16px 0;"">
                    Te informamos que tu reserva para el
                    <strong>{fecha}</strong> fue <strong>cancelada</strong>.
                  </p>

                  <p style=""margin:0;"">
                    Si creés que se trata de un error o querés reprogramar,
                    podés agendar una nueva reserva desde la plataforma.
                  </p>

                  {footer}
                </div>

              </div>
            </div>";

        await _enviarEmail.Ejecutar(cliente.Email, "Winni Electricidad - Reserva cancelada", cuerpoCliente, cancellationToken);

 
        var admin = await _repositorioUsuario.ObtenerAdministrador(cancellationToken);
        
        var cuerpoAdmin = $@"
          <div style=""font-family: Arial, sans-serif; background-color:#f4f6f8; padding:24px;"">
            <div style=""max-width:600px; margin:0 auto; background-color:#ffffff; border-radius:8px; overflow:hidden;"">

              <!-- Header -->
              <div style=""background-color:#1f3a5f; color:#ffffff; padding:16px 24px;"">
                <h2 style=""margin:0; font-size:20px;"">Winni Electricidad</h2>
              </div>

              <!-- Body -->
              <div style=""padding:24px; color:#333333;"">
                <h3 style=""margin-top:0; color:#1f3a5f;"">
                  Reserva cancelada
                </h3>

                <p style=""margin:0 0 8px 0;""><strong>Cliente:</strong> {cliente.NombreCompleto}</p>
                <p style=""margin:0 0 8px 0;""><strong>Email:</strong> {cliente.Email}</p>
                <p style=""margin:0 0 8px 0;""><strong>Teléfono:</strong> {cliente.Telefono}</p>

                <p style=""margin:16px 0 8px 0;""><strong>Tipo de servicio:</strong> {tipoServicio}</p>
                <p style=""margin:0 0 8px 0;""><strong>Dirección:</strong> {direccion}</p>
                <p style=""margin:0 0 8px 0;"">
                  <strong>Fecha de la reserva cancelada:</strong> {fecha}
                </p>

                <p style=""margin:16px 0 12px 0;"">
                  <strong>Comentario del cliente:</strong> {comentario}
                </p>

                <p style=""margin:0;"">
                  Esta reserva ha sido marcada como <strong>cancelada</strong> en el sistema.
                </p>

                {footer}
              </div>

            </div>
          </div>";

        await _enviarEmail.Ejecutar(admin.Email, "Reserva cancelada – Notificación interna", cuerpoAdmin, cancellationToken);
     }
}