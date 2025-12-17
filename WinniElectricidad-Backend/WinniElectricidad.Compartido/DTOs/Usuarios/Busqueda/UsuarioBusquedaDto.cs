using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Usuarios.Busqueda;

public class UsuarioBusquedaDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Telefono { get; set; }
}