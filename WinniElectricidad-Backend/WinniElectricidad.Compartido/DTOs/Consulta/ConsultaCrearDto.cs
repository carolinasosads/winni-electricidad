using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Consulta;

public class ConsultaCrearDto
{
    [Required]
    [MaxLength(120)]
    public string Nombre { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(160)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(30)]
    public string Telefono { get; set; } = null!;

    [Required]
    [MaxLength(2000)]
    public string Mensaje { get; set; } = null!;
}