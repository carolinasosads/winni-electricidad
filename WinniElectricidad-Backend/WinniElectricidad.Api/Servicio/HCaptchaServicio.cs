using Microsoft.Extensions.Options;
using WinniElectricidad.Compartido.DTOs.Seguridad;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios;

namespace WinniElectricidad.Api.Servicio;

public class HCaptchaServicio : IHCaptchaVerifier
{
    //va en api y no en logica aplicacion porque es infraestructura y no negocio, no hace logica solo le pega a una api
    private readonly HttpClient _http;
    private readonly HCaptchaOptions _opts;

    public HCaptchaServicio(HttpClient http, IOptions<HCaptchaOptions> options)
    {
        _http = http;
        _opts = options.Value;
    }

    public async Task<bool> VerifyAsync(string token, string? remoteIp = null, string? expectedHostname = null)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["secret"] = _opts.Secret,
            ["response"] = token,
        });

        using var resp = await _http.PostAsync("https://api.hcaptcha.com/siteverify", content);
        if (!resp.IsSuccessStatusCode) return false;

        var payload = await resp.Content.ReadFromJsonAsync<HCaptchaVerifyResponse>();
        if (payload is null || !payload.Success) return false;

        if (!string.IsNullOrWhiteSpace(expectedHostname) &&
            !string.Equals(expectedHostname, payload.Hostname, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        return true;
    }
}