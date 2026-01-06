namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Presupuesto
{
    #region  Propiedades
    public int Id { get; set; }
    public int IdReserva { get; set; }
    public Reserva Reserva { get; set; } = null!;
    public int IdUsuario { get; set; }
    public decimal Monto { get; set; }
    public decimal? MontoPagado { get; set; } 
    public string? DescripcionTrabajo { get; set; } 
    public string? Notas { get; set; }
    public DateTime FechaPresupuesto { get; set; }
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    #endregion

    public Presupuesto()
    {
        FechaPresupuesto = DateTime.UtcNow;
    }
}