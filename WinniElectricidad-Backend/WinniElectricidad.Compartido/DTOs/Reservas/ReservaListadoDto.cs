namespace WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;

public class ReservaListadoDto
{
    public int IdReserva { get; set; }

    public DateTime FechaReserva { get; set; }
    
    public string Estado { get; set; } = string.Empty;

    public string NombreServicio { get; set; } = string.Empty;

    public string DireccionDescripcion { get; set; } = string.Empty;

    public decimal MontoPresupuestado { get; set; } = 0;

    public bool TienePresupuesto { get; set; } = false;
}