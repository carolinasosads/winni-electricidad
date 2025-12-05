namespace WinniElectricidad.Compartido.Reservas;

public record ReservaCreadaDto
{
    public int IdReserva { get; set; }
    public DateTime FechaReserva { get; set; }
    public string TipoServicio { get; set; }
    public string Direccion { get; set; }
    public List<string> Servicios { get; set; } = new();
    public string? Comentario { get; set; }
}