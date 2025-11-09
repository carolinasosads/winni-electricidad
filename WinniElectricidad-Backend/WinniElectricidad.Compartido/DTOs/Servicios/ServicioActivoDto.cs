namespace WinniElectricidad.Compartido.DTOs.Servicios;

public record ServicioActivoDto
{
    public int Id { get; set; }
    public required string Titulo { get; set; }
}