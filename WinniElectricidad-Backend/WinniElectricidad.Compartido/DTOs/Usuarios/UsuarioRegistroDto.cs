using WinniElectricidad.Compartido.DTOs.Direcciones;

namespace WinniElectricidad.Compartido.DTOs.Usuarios;

public class UsuarioRegistroDto 
{
        public string NombreCompleto { get; set; }
        public string Email { get; set; } 
        public string Password { get; set; }
        public ICollection<DireccionDto?> Direcciones { get; set; } 
        public string? Telefono { get; set; }
        public string HCaptchaToken { get; set; }
}