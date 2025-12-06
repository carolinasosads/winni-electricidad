using System.Security.Cryptography;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class ServicioHash : IServicioHash
{
    private const int SaltSize = 32;
    private const int KeySize = 32;
    private const int Iterations = 150000;

    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize
        );

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool VerificarPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.', 3);
        if (parts.Length != 3)
            return false;

        try
        {
            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);
            
            var keyToCheck = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                key.Length
            );

            return CryptographicOperations.FixedTimeEquals(key, keyToCheck);
        }
        catch
        {
            return false;
        }
    }
}