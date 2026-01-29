namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Pago;

public interface IProcesarWebhookPagoMercadoPago
{
    Task ProcesarAsync(string? status, long mpPaymentId, string? externalReference, CancellationToken ct = default);
}