using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Pago;

public class PagoPendienteDto
{
    [Required] 
    public int IdPresupuesto { get; set; }
    [Required(ErrorMessage = "El monto del pago es obligatorio.")]
    public decimal Monto { get; set; }
    [Required] public List<string> NombresServicios { get; set; } = new List<string>();
    [Required]
    [Range(0, double.MaxValue)]
    public decimal MontoTotal { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? MontoPagado { get; set; }
}