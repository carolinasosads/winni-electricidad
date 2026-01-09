using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Consulta;

public class ConsultaCrearDto
{
    public int? IdCliente { get; set; }

    [MaxLength(120)]
    public string? Nombre { get; set; }

    [EmailAddress]
    [MaxLength(160)]
    public string? Email { get; set; }

    [MaxLength(30)]
    public string? Telefono { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Mensaje { get; set; } = "";
}