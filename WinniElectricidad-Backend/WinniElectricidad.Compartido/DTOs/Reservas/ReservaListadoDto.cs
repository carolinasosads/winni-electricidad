using System.ComponentModel.DataAnnotations;

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

    [Required]
    public string DireccionDescripcion { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal MontoPresupuestado { get; set; } = 0;

    [Required]
    public bool TienePresupuesto { get; set; } = false;
}