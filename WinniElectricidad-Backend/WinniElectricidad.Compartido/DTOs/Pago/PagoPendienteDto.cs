using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Pago;

public class PagoPendienteDto
{
    [Required] 
    public int IdPresupuesto { get; set; }
    [Required(ErrorMessage = "El monto del pago es obligatorio.")]
    public decimal Monto { get; set; }
}