namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios;

public interface IRecuperarContrasena
{
    Task EnviarCorreoRecuperacion(string email, CancellationToken ct);
    Task ResetearContrasena(string password, string token, CancellationToken ct);
}