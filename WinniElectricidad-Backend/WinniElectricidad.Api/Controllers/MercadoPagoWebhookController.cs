using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Pago;
namespace WinniElectricidad.Api.Controllers;

[ApiController]
[Route("WinniElectricidadApi/mercadopago")]
public class MercadoPagoWebhookController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProcesarWebhookPagoMercadoPago _procesarWebhook;
    private readonly IHostEnvironment _env;

    public MercadoPagoWebhookController(IConfiguration config, IHttpClientFactory httpClientFactory, IProcesarWebhookPagoMercadoPago procesarWebhook, IHostEnvironment env)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;
        _procesarWebhook = procesarWebhook;
        _env = env;
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        var dataId = HttpContext.Request.Query["data.id"].ToString();
        if (string.IsNullOrWhiteSpace(dataId))
            dataId = HttpContext.Request.Query["id"].ToString();

        var type = HttpContext.Request.Query["type"].ToString();
        if (string.IsNullOrWhiteSpace(type))
            type = HttpContext.Request.Query["topic"].ToString();

        if (string.IsNullOrWhiteSpace(dataId) || string.IsNullOrWhiteSpace(type))
        {
            HttpContext.Request.EnableBuffering();

            using var reader = new StreamReader(HttpContext.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync(ct);
            HttpContext.Request.Body.Position = 0;

            if (!string.IsNullOrWhiteSpace(body))
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                if (string.IsNullOrWhiteSpace(type) && root.TryGetProperty("type", out var typeProp))
                    type = typeProp.GetString() ?? "";

                if (string.IsNullOrWhiteSpace(dataId) &&
                    root.TryGetProperty("data", out var dataProp) &&
                    dataProp.ValueKind == JsonValueKind.Object &&
                    dataProp.TryGetProperty("id", out var idProp))
                {
                    dataId = idProp.GetString() ?? "";
                }
            }
        }

        if (string.IsNullOrWhiteSpace(dataId) || string.IsNullOrWhiteSpace(type))
        {
            Console.WriteLine("Webhook vacío o inválido.");
            return Ok();
        }

        var validarFirma = _config.GetValue<bool>("MercadoPago:ValidarFirma", true);

        if (validarFirma && !_env.IsDevelopment())
        {
            if (!ValidarFirma(dataId)) return Unauthorized();
        }

        if (type == "payment")
        {
            await ProcesarPago(dataId, ct);
        }

        return Ok();
    }

    private bool ValidarFirma(string dataId)
    {
        var secret = _config["MercadoPago:WebhookSecret"];
        if (string.IsNullOrWhiteSpace(secret))
            return false;

        var xSignature = Request.Headers["X-Signature"].ToString();
        var xRequestId = Request.Headers["X-Request-Id"].ToString();

        if (string.IsNullOrWhiteSpace(xSignature) || string.IsNullOrWhiteSpace(xRequestId))
            return false;

        var parts = xSignature.Split(',');
        string? ts = null;
        string? v1 = null;

        foreach (var part in parts)
        {
            var kv = part.Split('=', 2);
            if (kv.Length != 2) continue;
            if (kv[0] == "ts") ts = kv[1];
            if (kv[0] == "v1") v1 = kv[1];
        }

        if (string.IsNullOrWhiteSpace(ts) || string.IsNullOrWhiteSpace(v1))
            return false;

        var manifest = $"id:{dataId};request-id:{xRequestId};ts:{ts};";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(manifest));
        var computed = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        return computed == v1;
    }

    private async Task ProcesarPago(string dataId, CancellationToken ct)
    {
        var accessToken = _config["MercadoPago:AccessToken"];
        if (string.IsNullOrWhiteSpace(accessToken))
            return;

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var res = await client.GetAsync($"https://api.mercadopago.com/v1/payments/{dataId}", ct);
        if (!res.IsSuccessStatusCode)
            return;

        var body = await res.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        long mpPaymentId = root.GetProperty("id").GetInt64();
        var status = root.GetProperty("status").GetString();

        string? externalReference = null;
        if (root.TryGetProperty("external_reference", out var erProp) && erProp.ValueKind != JsonValueKind.Null)
            externalReference = erProp.GetString();

        await _procesarWebhook.ProcesarAsync(status, mpPaymentId, externalReference, ct);
    }
}
