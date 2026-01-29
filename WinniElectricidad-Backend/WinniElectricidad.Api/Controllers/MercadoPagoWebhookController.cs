using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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
            Console.WriteLine("Webhook vacío o inválido.");
            return Ok();
        }

        if (!_env.IsDevelopment())
        {
            if (!ValidarFirma(dataId)) return Unauthorized();
        }
        else
        {
            Console.WriteLine("MODO: Desarrollo (Firma omitida)");
        }

        if (type == "payment")
        {
            await ProcesarPago(dataId, ct);
        }

        return Ok();
    }

    private async Task ProcesarPago(string dataId, CancellationToken ct)
    {
        var accessToken = _config["MercadoPago:AccessToken"];
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var res = await client.GetAsync($"https://api.mercadopago.com/v1/payments/{dataId}", ct);
        if (!res.IsSuccessStatusCode)
        {
            return;
        }

        var body = await res.Content.ReadAsStringAsync(ct);

        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        long mpPaymentId = root.GetProperty("id").ValueKind == JsonValueKind.Number 
            ? root.GetProperty("id").GetInt64() 
            : long.Parse(root.GetProperty("id").GetString() ?? "0");

        var status = root.GetProperty("status").GetString();
        
        string? externalReference = null;
        if (root.TryGetProperty("external_reference", out var erProp) && erProp.ValueKind != JsonValueKind.Null)
        {
            externalReference = erProp.GetString();
        }
        
        await _procesarWebhook.ProcesarAsync(status, mpPaymentId, externalReference, ct);
    }

    private bool ValidarFirma(string dataId)
    {
        var xSignature = HttpContext.Request.Headers["X-Signature"].ToString();
        var xRequestId = HttpContext.Request.Headers["X-Request-Id"].ToString();

        if (string.IsNullOrWhiteSpace(xSignature) || string.IsNullOrWhiteSpace(xRequestId)) return false;
        if (!TryParseSignature(xSignature, out var ts, out var v1)) return false;

        var manifest = $"id:{dataId};request-id:{xRequestId};ts:{ts};";
        var secret = _config["MercadoPago:WebhookSecret"];
        
        if (string.IsNullOrWhiteSpace(secret)) return false;

        var computed = ComputeHmacSha256Hex(manifest, secret);
        return CryptographicEquals(computed, v1);
    }

    private static bool TryParseSignature(string xSignature, out string ts, out string v1)
    {
        ts = ""; v1 = "";
        var parts = xSignature.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var p in parts)
        {
            var kv = p.Split('=', 2, StringSplitOptions.TrimEntries);
            if (kv.Length == 2)
            {
                if (kv[0] == "ts") ts = kv[1];
                if (kv[0] == "v1") v1 = kv[1];
            }
        }
        return !string.IsNullOrWhiteSpace(ts) && !string.IsNullOrWhiteSpace(v1);
    }

    private static string ComputeHmacSha256Hex(string data, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    private static bool CryptographicEquals(string a, string b)
    {
        if (a.Length != b.Length) return false;
        int result = 0;
        for (int i = 0; i < a.Length; i++) result |= a[i] ^ b[i];
        return result == 0;
    }
}