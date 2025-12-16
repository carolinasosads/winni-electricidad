using System.ComponentModel.DataAnnotations;
using WinniElectricidad.Compartido.DTOs.Direcciones;

namespace WinniElectricidad.Compartido.DTOs.Registro;

public class UsuarioRegistroAdminDto
{
    [Required]
    [MaxLength(100)]
    public string NombreCompleto { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; } 

    [Required]
    public ICollection<DireccionDto> Direcciones { get; set; }

    [Phone]
    public string? Telefono { get; set; }
}