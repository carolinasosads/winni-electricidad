namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface IRecuperarContrasena
{
    Task EnviarCorreoRecuperacion(string email, CancellationToken ct = default);
    Task ResetearContrasena(string password, string token, CancellationToken ct = default);
}