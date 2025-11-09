using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Usuarios.RecuperacionContrasena;

public class UsuarioResetPasswordDto
{
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public required string Password { get; set; }
    
    [Required(ErrorMessage = "El token es obligatorio.")]
    public required string TokenPlain { get; set; }
}