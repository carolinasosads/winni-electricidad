using System.Security.Cryptography;
using System.Text;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class ServicioOneTimeToken : IServicioOneTimeToken
{
    public (string tokenPlain, string tokenHash) Create(int bytes = 32)
    {
        var plain = Generate(bytes);
        return (plain, Hash(plain));
    }

    public string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    private static string Generate(int bytes)
    {
        var b = RandomNumberGenerator.GetBytes(bytes);
        return Convert.ToBase64String(b).Replace('+','-').Replace('/','_').TrimEnd('=');
    }
}