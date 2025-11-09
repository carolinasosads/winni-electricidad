namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface IHCaptchaVerifier
{
    Task<bool> VerifyAsync(string token, string? remoteIp = null, string? expectedHostname = null);
}