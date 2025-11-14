using System.ComponentModel.DataAnnotations;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.Reservas;

public class ReservaACrearDto
{
    [Required]
    public DateTime FechaReserva { get; set; }

    [Required]
    public TipoServicioReserva TipoServicio { get; set; }  

    [Required]
    public int IdDireccion { get; set; }

    [Required]
    public List<int> IdServicios { get; set; } = [];

    public string? Comentario { get; set; }
}