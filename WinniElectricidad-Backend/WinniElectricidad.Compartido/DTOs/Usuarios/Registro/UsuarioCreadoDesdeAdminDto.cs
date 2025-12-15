using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Registro;

public class UsuarioCreadoDesdeAdminDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}