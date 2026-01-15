using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reserva;

public class ModificarReserva : IModificarReserva
{
    private readonly IEnviarEmail _enviarEmail;
    private readonly IRepositorioReserva _repositorioReserva;
    private readonly IRepositorioUsuario _repositorioUsuario;

    public ModificarReserva(IRepositorioReserva repositorioReserva, IEnviarEmail enviarEmail, IRepositorioUsuario repositorioUsuario)
    {
        _repositorioReserva = repositorioReserva;
        _enviarEmail = enviarEmail;
        _repositorioUsuario = repositorioUsuario;
    }

    public async Task Ejecutar(ReservaAModificarDto dto, bool esAdmin, CancellationToken cancellationToken = default)
    {
        var reserva = await _repositorioReserva.ObtenerReservaPorId(dto.IdReserva, cancellationToken);

        if (reserva is null)
            throw new ArgumentException("La reserva no existe.");

        var fechaAnterior = reserva.FechaReserva;

        var desde = dto.NuevaFecha.AddMinutes(-5);
        var hasta = dto.NuevaFecha.AddMinutes(5);

        var reservasEnRango = await _repositorioReserva
            .FindAllBetweenDates(desde, hasta, cancellationToken);

        if (esAdmin)
        {
            reserva.MarcarPendientePorCambioDeAdmin(dto.NuevaFecha, reservasEnRango);
        }
        else
        {
            reserva.RequiereConfirmacionCliente = false;
            reserva.Reprogramar(dto.NuevaFecha, reservasEnRango);
        }

        await _repositorioReserva.Update(reserva, cancellationToken);

        var cliente = reserva.UsuarioCliente
                      ?? throw new InvalidOperationException("El cliente asociado a la reserva no existe.");

        var fechaAnteriorString = fechaAnterior.ToString("dd/MM/yyyy HH:mm");
        var fechaNuevaString = reserva.FechaReserva.ToString("dd/MM/yyyy HH:mm");

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

        var bloqueEstadoCliente = (esAdmin && reserva.RequiereConfirmacionCliente)
            ? @"
                <p>
                    Esta nueva fecha es una <strong>sugerida</strong> y quedará pendiente de tu confirmación.
                </p>
                <p>
                    Tu reserva volverá a estado <strong>Pendiente</strong> hasta que confirmemos juntos la nueva fecha.
                </p>
                <p>
                    Ante cualquier duda o ajuste que quieras realizar, podés responder desde tu panel de reservas.
                </p>"
            : @"
                <p>
                    La reserva quedó <strong>Pendiente</strong> para su revisión.
                </p>";

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
                  <h3 style=""margin-top:0; color:#1f3a5f;"">Reserva reprogramada</h3>

                  <p style=""margin:0 0 12px 0;"">
                    Hola <strong>{cliente.NombreCompleto} 👋🏽</strong>,
                  </p>

                  <p style=""margin:0 0 16px 0;"">
                    Te informamos que tu reserva fue
                    <strong>reprogramada con una nueva fecha sugerida</strong>.
                  </p>

                  <p style=""margin:0 0 8px 0;"">
                    <strong>Fecha anterior:</strong> {fechaAnteriorString}
                  </p>
                  <p style=""margin:0 0 16px 0;"">
                    <strong>Nueva fecha sugerida:</strong> {fechaNuevaString}
                  </p>

                  {bloqueEstadoCliente}

                  {footer}
                </div>

              </div>
            </div>";

        await _enviarEmail.Ejecutar(cliente.Email, "Winni Electricidad - Reserva reprogramada", cuerpoCliente, cancellationToken);

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
                    Reserva reprogramada (nueva fecha sugerida)
                  </h3>

                  <p style=""margin:0 0 8px 0;""><strong>Cliente:</strong> {cliente.NombreCompleto}</p>
                  <p style=""margin:0 0 8px 0;""><strong>Email:</strong> {cliente.Email}</p>
                  <p style=""margin:0 0 8px 0;""><strong>Teléfono:</strong> {cliente.Telefono}</p>

                  <p style=""margin:16px 0 8px 0;""><strong>Tipo de servicio:</strong> {tipoServicio}</p>
                  <p style=""margin:0 0 8px 0;""><strong>Dirección:</strong> {direccion}</p>

                  <p style=""margin:16px 0 8px 0;"">
                    <strong>Fecha anterior:</strong> {fechaAnteriorString}
                  </p>
                  <p style=""margin:0 0 12px 0;"">
                    <strong>Nueva fecha sugerida:</strong> {fechaNuevaString}
                  </p>

                  <p style=""margin:0 0 16px 0;"">
                    <strong>Comentario del cliente:</strong> {comentario}
                  </p>

                  <p style=""margin:0;"">
                    Recordá contactar al cliente para confirmar si la nueva fecha le queda bien
                    o coordinar una alternativa.
                  </p>

                  {footer}
                </div>

              </div>
            </div>";
        await _enviarEmail.Ejecutar(admin.Email, "Reserva reprogramada – Nueva fecha sugerida", cuerpoAdmin, cancellationToken);
    }
}
