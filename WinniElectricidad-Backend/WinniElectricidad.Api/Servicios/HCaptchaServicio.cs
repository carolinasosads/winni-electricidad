using Microsoft.Extensions.Options;
using WinniElectricidad.Compartido.DTOs.Seguridad;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

namespace WinniElectricidad.Api.Servicios;

/// <summary>
/// Servicio responsable de verificar los tokens de <b>hCaptcha</b> enviados desde el frontend.
/// </summary>
/// <remarks>
/// Este servicio se comunica con la API oficial de hCaptcha para validar que la respuesta enviada por el cliente
/// sea legítima y haya sido generada por un humano.  
///
/// Se encuentra en la capa <b>API</b> (infraestructura) porque su responsabilidad es interactuar con un servicio externo,
/// no ejecutar lógica de negocio.
///
/// Este servicio implementa la interfaz <see cref="IHCaptchaVerifier"/>.
/// </remarks>
public class HCaptchaServicio : IHCaptchaVerifier
{
    private readonly HttpClient _http;
    private readonly HCaptchaOptions _opts;

    /// <summary>
    /// Inicializa una nueva instancia de <see cref="HCaptchaServicio"/> con las dependencias necesarias.
    /// </summary>
    /// <param name="http">Instancia de <see cref="HttpClient"/> utilizada para realizar la solicitud HTTP a la API de hCaptcha.</param>
    /// <param name="options">Configuración de <see cref="HCaptchaOptions"/> que contiene la clave secreta (<c>secret</c>) para la verificación.</param>
    public HCaptchaServicio(HttpClient http, IOptions<HCaptchaOptions> options)
    {
        _http = http;
        _opts = options.Value;
    }

    /// <summary>
    /// Verifica la validez de un token de hCaptcha comunicándose con la API de verificación oficial.
    /// </summary>
    /// <param name="token">
    /// Token generado por el widget de hCaptcha en el cliente.  
    /// Es enviado por el frontend tras una validación exitosa del usuario.
    /// </param>
    /// <param name="remoteIp">
    /// (Opcional) Dirección IP del cliente. Puede incluirse para reforzar la validación del desafío.
    /// </param>
    /// <param name="expectedHostname">
    /// (Opcional) Nombre de host esperado donde se originó la validación.  
    /// Si se especifica, la verificación también confirmará que el hostname recibido por hCaptcha coincida.
    /// </param>
    /// <returns>
    /// Un valor booleano que indica el resultado de la verificación:
    /// <list type="bullet">
    /// <item><description><c>true</c>: El token es válido y pertenece al dominio esperado.</description></item>
    /// <item><description><c>false</c>: El token es inválido, expiró o pertenece a otro dominio.</description></item>
    /// </list>
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Se produce si la solicitud a la API de hCaptcha falla (por ejemplo, problemas de red).
    /// </exception>
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