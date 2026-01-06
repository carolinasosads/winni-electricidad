using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Consulta;

public class ConsultaCrearDto
{

    [Required]
    [MaxLength(120)]
    public string Nombre { get; set; } = "";

    [Required]
    [EmailAddress]
    [MaxLength(160)]
    public string Email { get; set; } = "";
    
    [Required]
    [MaxLength(30)]
    public string? Telefono { get; set; }
    
    [Required]
    [MaxLength(2000)]
    public string Mensaje { get; set; } = "";

}