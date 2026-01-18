using System.ComponentModel.DataAnnotations;
using WinniElectricidad.Compartido.Reservas;

namespace WinniElectricidad.Compartido.DTOs.Presupuesto;

public class PresupuestoConReservaDto
{
    [Required] 
    public int Id { get; set; }
    
    public ReservaCreadaDto reserva { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal MontoTotal { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal? MontoPagado { get; set; }
    
    [MaxLength(500, ErrorMessage = "La descripción del trabajo no puede superar los 500 caracteres.")]
    public string? DescripcionTrabajo { get; set; }

    [Required]
    public DateTime FechaCreacion { get; set; }
}