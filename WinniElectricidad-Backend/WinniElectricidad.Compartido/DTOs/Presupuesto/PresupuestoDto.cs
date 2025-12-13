namespace WinniElectricidad.Compartido.DTOs.Presupuesto;

public class PresupuestoDto
{
    public int Id { get; set; }
    public int IdReserva { get; set; }
    public decimal MontoTotal { get; set; }
    public DateTime FechaCreacion { get; set; }
}