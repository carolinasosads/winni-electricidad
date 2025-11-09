using System.ComponentModel.DataAnnotations;
using WinniElectricidad.Compartido.DTOs.Direcciones;

namespace WinniElectricidad.Compartido.DTOs.Usuarios;

public class UsuarioRegistroDto 
{
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre completo no puede superar los 100 caracteres.")]
        public string NombreCompleto { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string Email { get; set; } 
        
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Debe ingresar al menos una dirección.")]
        public ICollection<DireccionDto?> Direcciones { get; set; }
        
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido.")]
        public string? Telefono { get; set; }
        
        [Required(ErrorMessage = "Debe validar el hCaptcha.")]
        public string HCaptchaToken { get; set; }
}