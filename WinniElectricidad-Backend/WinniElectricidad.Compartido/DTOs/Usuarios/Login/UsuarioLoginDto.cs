using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Usuarios.Login;

public class UsuarioLoginDto
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    public required string Email { get; set; }
    
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public required string Password { get; set; }
}