using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;

namespace WinniElectricidad.Compartido.DTOs.Reseñas;

public class ReseñaCreadaDto
{
    public int IdReseña { get; set; }
    public string Descripcion  { get; set; }
    public int Calificacion { get; set; }
    public DateTime FechaReseña { get; set; }
    public UsuarioReservaDto Cliente { get; set; } // TODO: Modificar este dto para que sea mas general capaz, para no duplicar logica
    public ServicioActivoDto Servicio { get; set; }
    public string? ImagenUrl { get; set; }
}