using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Pago;

public class PagoCrearDto
{
    [Required(ErrorMessage = "El monto del pago es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto del pago debe ser mayor a 0.")]
    public decimal Monto { get; set; }
}