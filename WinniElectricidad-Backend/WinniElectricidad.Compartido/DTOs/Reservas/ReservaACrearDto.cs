using System.ComponentModel.DataAnnotations;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.Reservas;

public class ReservaACrearDto
{
    [Required(ErrorMessage = "Debe seleccionar una fecha y hora para la reserva.")]
    public DateTime FechaReserva { get; set; }

    [Required(ErrorMessage = "Debe seleccionar el tipo de servicio.")]
    public TipoServicioReserva TipoServicio { get; set; }  

    [Required(ErrorMessage = "Debe seleccionar una dirección.")]
    [Range(1, int.MaxValue, ErrorMessage = "El Id de la dirección no es válido.")]
    public int IdDireccion { get; set; }

    [Required(ErrorMessage = "Debe seleccionar al menos un servicio.")]
    [MinLength(1, ErrorMessage = "Debe seleccionar al menos un servicio.")]
    public List<int> IdServicios { get; set; } = [];

    [MaxLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
    public string? Comentario { get; set; }

}