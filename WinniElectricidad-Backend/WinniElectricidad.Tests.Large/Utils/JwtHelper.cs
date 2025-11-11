using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace WinniElectricidad.Tests.Large.Utils;

/// <summary>
/// Helper para generar tokens JWT en los tests Large,
/// utilizando la configuración real del appsettings.Testing.json.
/// </summary>
public class JwtHelper
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtHelper(IConfiguration configuration)
    {
        _key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("No se encontró Jwt:Key en la configuración.");
        _issuer = configuration["Jwt:Issuer"] ?? "WinniElectricidad.Testing";
        _audience = configuration["Jwt:Audience"] ?? "WinniElectricidad.Testing";
    }

    /// <summary>
    /// Genera un token válido para los tests
    /// </summary>
    public string GenerarTokenValido(int idUsuario, string email, string rol, int expHoras = 2)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.ASCII.GetBytes(_key);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, rol)
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(expHoras),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(descriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Genera un token expirado 
    /// </summary>
    public string GenerarTokenExpirado(int idUsuario, string email, string rol)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.ASCII.GetBytes(_key);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, rol)
            ]),
            NotBefore = DateTime.UtcNow.AddHours(-2),
            Expires = DateTime.UtcNow.AddHours(-1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(descriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Genera un token válido pero con un rol distinto
    /// </summary>
    public string GenerarTokenConRol(int idUsuario, string email, string rol)
    {
        return GenerarTokenValido(idUsuario, email, rol);
    }
}