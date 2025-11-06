namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios;

public interface IServicioOneTimeToken
{
    (string tokenPlain, string tokenHash) Create(int bytes = 32);
    string Hash(string tokenPlain);
}