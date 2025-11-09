namespace WinniElectricidad.Compartido.DTOs.Servicios;

public record ServicioDisponibleDto
{
    public int Id { get; set; }
    public required string Titulo { get; set; }
}