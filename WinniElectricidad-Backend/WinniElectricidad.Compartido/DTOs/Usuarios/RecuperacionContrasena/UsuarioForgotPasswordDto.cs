using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Usuarios.RecuperacionContrasena;

public class UsuarioForgotPasswordDto
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
    public required string Email { get; set; }
}