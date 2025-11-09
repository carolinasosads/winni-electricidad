namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface IServicioToken
{
    string GenerarToken(int idUsuario, string email, string rol);
}