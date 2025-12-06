namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface IServicioHash
{
    string Hash(string passwordPlain);
    bool VerificarPassword(string password, string hashedPassword);
}