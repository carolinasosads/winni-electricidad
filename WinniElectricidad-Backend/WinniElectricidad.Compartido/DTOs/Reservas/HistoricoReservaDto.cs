using WinniElectricidad.Compartido.DTOs.Direcciones;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;

namespace WinniElectricidad.Compartido.DTOs.Reservas;

public class HistoricoReservaDto
{
    public int IdReserva { get; set; }
    public DateTime FechaReserva { get; set; }
    public string Estado { get; set; } = null!;
    public string TipoServicio { get; set; } = null!;
    public string? Comentario { get; set; }

    public UsuarioReservaDto Cliente { get; set; } = null!;
    
    public DireccionDto Direccion { get; set; } = null!;
    
    public ICollection<ServicioActivoDto> Servicios { get; set; } = new List<ServicioActivoDto>();

}