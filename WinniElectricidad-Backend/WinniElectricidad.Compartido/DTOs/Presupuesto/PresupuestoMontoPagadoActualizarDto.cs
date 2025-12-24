using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Presupuesto;

public class PresupuestoMontoPagadoActualizarDto
{
    [Required(ErrorMessage = "El monto pagado es obligatorio.")]
    [Range(0, double.MaxValue, ErrorMessage = "El monto pagado no puede ser negativo.")]
    public decimal MontoPagado { get; set; }
}