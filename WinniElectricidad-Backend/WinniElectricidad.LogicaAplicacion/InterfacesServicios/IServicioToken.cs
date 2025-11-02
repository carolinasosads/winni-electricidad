namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios;

public interface IServicioToken
{
    string GenerarToken(int idUsuario, string email, string rol);
}