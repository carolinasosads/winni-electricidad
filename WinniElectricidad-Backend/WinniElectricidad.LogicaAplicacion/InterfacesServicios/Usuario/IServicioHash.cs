namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface IServicioHash
{
    string Hash(string password);
    bool VerificarPassword(string password, string hashedPassword);
}