namespace WinniElectricidad.Compartido.DTOs.Direcciones;

public class DireccionDetalleDto
{
    public int Id { get; set; }
    public required string Calle { get; set; }
    public required string Esquina { get; set; }
    public string? Numero { get; set; }
    public string? Apto { get; set; }
}