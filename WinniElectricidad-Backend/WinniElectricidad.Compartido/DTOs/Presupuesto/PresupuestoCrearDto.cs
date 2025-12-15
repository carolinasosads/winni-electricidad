using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Presupuesto;

public class PresupuestoCrearDto
{
    [Required(ErrorMessage = "El monto total es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto total debe ser mayor a 0.")]
    public decimal MontoTotal { get; set; }
    [MaxLength(500, ErrorMessage = "Las notas internas no pueden superar los 500 caracteres.")]
    public string? NotasInternas { get; set; }
}