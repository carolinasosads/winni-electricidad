using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Presupuesto;

public class PresupuestoDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int IdReserva { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal MontoTotal { get; set; }

    [Required]
    public DateTime FechaCreacion { get; set; }
}