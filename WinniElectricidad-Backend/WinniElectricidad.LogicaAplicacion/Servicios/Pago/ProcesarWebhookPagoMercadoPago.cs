using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Pago;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Pago;

public class ProcesarWebhookPagoMercadoPago : IProcesarWebhookPagoMercadoPago
{
    private readonly IRepositorioPago _repoPago;
    private readonly IRepositorioPresupuesto _repoPresupuesto;
    private readonly IRepositorioUsuario _repoUsuario;
    private readonly IEnviarEmail _enviarEmail;

    public ProcesarWebhookPagoMercadoPago(IRepositorioPago repoPago, IRepositorioPresupuesto repoPresupuesto, IRepositorioUsuario repoUsuario, IEnviarEmail enviarEmail)
    {
        _repoPago = repoPago;
        _repoPresupuesto = repoPresupuesto;
        _repoUsuario = repoUsuario;
        _enviarEmail = enviarEmail;
    }

    public async Task ProcesarAsync(string? status, long mpPaymentId, string? externalReference, CancellationToken ct = default)
    {
        if (!int.TryParse(externalReference, out var idPagoLocal)) return;

        var pago = await _repoPago.FindById(idPagoLocal, ct);
        if (pago is null) return;

        var estadoAnterior = pago.EstadoPago;

        pago.MercadoPagoPaymentId = mpPaymentId;
        pago.EstadoPago = MapearEstado(status ?? "approved");

        await _repoPago.Update(pago, ct);

        if (pago.EstadoPago == EstadoPago.Confirmado)
        {
            var presupuesto = await _repoPresupuesto.FindById(pago.IdPresupuesto, ct);
            if (presupuesto != null)
            {
                presupuesto.MontoPagado = presupuesto.Pagos
                    .Where(p => p.EstadoPago == EstadoPago.Confirmado)
                    .Sum(p => p.Monto);

                await _repoPresupuesto.Update(presupuesto, ct);
            }

            if (estadoAnterior != EstadoPago.Confirmado)
            {
                await NotificarPagoExitoso(pago, ct);
            }
        }
    }

    private async Task NotificarPagoExitoso(LogicaNegocio.Entidades.Pago pago, CancellationToken ct)
    {
        var admin = await _repoUsuario.ObtenerAdministrador(ct);

        var cliente = await _repoUsuario.FindById(pago.IdUsuario, ct);
        if (cliente is null) return;

        var footer = _enviarEmail.GetFooter();
        var monto = pago.Monto.ToString("0.00");
        var fecha = pago.FechaHoraRealizado.ToString("dd/MM/yyyy HH:mm");

        // Email Cliente
        var cuerpoCliente = $@"
          <div style=""font-family: Arial, sans-serif; background-color:#f4f6f8; padding:24px;"">
            <div style=""max-width:600px; margin:0 auto; background-color:#ffffff; border-radius:8px; overflow:hidden;"">
              <div style=""background-color:#1f3a5f; color:#ffffff; padding:16px 24px;"">
                <h2 style=""margin:0; font-size:20px;"">Winni Electricidad</h2>
              </div>
              <div style=""padding:24px; color:#333333;"">
                <h3 style=""margin-top:0; color:#1f3a5f;"">Pago exitoso </h3>

                <p style=""margin:0 0 12px 0;"">
                  Hola <strong>{cliente.NombreCompleto}</strong> 👋🏽,
                </p>

                <p style=""margin:0 0 12px 0;"">
                  Recibimos tu pago correctamente.
                </p>

                <p style=""margin:0 0 6px 0;""><strong>Monto:</strong> ${monto}</p>
                <p style=""margin:0 0 6px 0;""><strong>Fecha:</strong> {fecha}</p>

                {footer}
              </div>
            </div>
          </div>";

        await _enviarEmail.Ejecutar(cliente.Email, "Winni Electricidad - Pago exitoso", cuerpoCliente, ct);
        await Task.Delay(TimeSpan.FromSeconds(2), ct);

        // Email Admin
        var cuerpoAdmin = $@"
          <div style=""font-family: Arial, sans-serif; background-color:#f4f6f8; padding:24px;"">
            <div style=""max-width:600px; margin:0 auto; background-color:#ffffff; border-radius:8px; overflow:hidden;"">
              <div style=""background-color:#1f3a5f; color:#ffffff; padding:16px 24px;"">
                <h2 style=""margin:0; font-size:20px;"">Winni Electricidad</h2>
              </div>
              <div style=""padding:24px; color:#333333;"">
                <h3 style=""margin-top:0; color:#1f3a5f;"">Pago confirmado (Interno)</h3>

                <p style=""margin:0 0 8px 0;""><strong>Cliente:</strong> {cliente.NombreCompleto}</p>
                <p style=""margin:0 0 8px 0;""><strong>Email:</strong> {cliente.Email}</p>
                <p style=""margin:0 0 8px 0;""><strong>Monto:</strong> ${monto}</p>
                <p style=""margin:0 0 16px 0;""><strong>Fecha:</strong> {fecha}</p>
                <p style=""margin:0 0 16px 0;""><strong>MP PaymentId:</strong> {pago.MercadoPagoPaymentId}</p>

                {footer}
              </div>
            </div>
          </div>";

        await _enviarEmail.Ejecutar(admin.Email, "Pago confirmado – Notificación interna", cuerpoAdmin, ct);
    }

    private static EstadoPago MapearEstado(string status)
    {
        status = status.Trim().ToLowerInvariant();

        return status switch
        {
            "approved" => EstadoPago.Confirmado,

            "pending" => EstadoPago.Pendiente,
            "in_process" => EstadoPago.Pendiente,

            "rejected" => EstadoPago.Fallido,
            "cancelled" => EstadoPago.Fallido,

            "refunded" => EstadoPago.Fallido,
            "charged_back" => EstadoPago.Fallido,

            _ => EstadoPago.Pendiente
        };
    }
}