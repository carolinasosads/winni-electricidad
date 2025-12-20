using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Presupuesto;

public class PresupuestoDto
{
    [Required] public int Id { get; set; }

    [Required] public int IdReserva { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal MontoTotal { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal? MontoPagado { get; set; }
    
    [MaxLength(500, ErrorMessage = "La descripciòn del trabajo no puede superar los 500 caracteres.")]
    public string? DescripcionTrabajo { get; set; }
    
    [MaxLength(500, ErrorMessage = "Las notas internas no pueden superar los 500 caracteres.")]
    public string? Notas { get; set; }


[Required]
    public DateTime FechaCreacion { get; set; }
}