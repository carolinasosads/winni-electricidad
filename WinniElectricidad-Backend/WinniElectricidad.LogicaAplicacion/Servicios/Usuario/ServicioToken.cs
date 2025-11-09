using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class ServicioToken : IServicioToken
{
    private readonly IConfiguration _configuration;

    public ServicioToken(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string GenerarToken(int idUsuario, string email, string rol)
    {
        var key = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var claveCodificada = Encoding.ASCII.GetBytes(key ?? throw new InvalidOperationException("JWT Key no configurada. Verificar appsettings o variables de entorno."));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, rol)
            ]),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(claveCodificada),
                SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}