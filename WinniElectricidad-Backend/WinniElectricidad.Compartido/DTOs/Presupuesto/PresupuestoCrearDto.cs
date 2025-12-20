using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Presupuesto;

public class PresupuestoCrearDto
{
    [Required(ErrorMessage = "El monto total es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto total debe ser mayor a 0.")]
    public decimal MontoTotal { get; set; }
    
    [MaxLength(500, ErrorMessage = "Las notas internas no pueden superar los 500 caracteres.")]
    public string? NotasInternas { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "El monto pagado no puede ser negativo.")]
    public decimal? MontoPagado { get; set; }
    
    [MaxLength(500, ErrorMessage = "La descripción del trabajo no puede superar los 500 caracteres.")]
    public string? DescripcionTrabajo { get; set; }
}
