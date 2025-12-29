using System.ComponentModel.DataAnnotations;
using WinniElectricidad.Compartido.DTOs.Direcciones;

namespace WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;

public class ReservaListadoDto
{
    [Required]
    public int IdReserva { get; set; }

    [Required]
    public DateTime FechaReserva { get; set; }

    [Required]
    public string Estado { get; set; } = string.Empty;

    [Required]
    public string NombreServicio { get; set; } = string.Empty;
    
    [Range(0, double.MaxValue)]
    public decimal MontoPresupuestado { get; set; } = 0;

    [Required]
    public bool TienePresupuesto { get; set; } = false;
    
    public DireccionDto? Direccion { get; set; }
    
    [Required]
    public List<string> Servicios { get; set; } = new();
    
    public bool RequiereConfirmacionCliente { get; set; } = false;

}