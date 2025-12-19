namespace WinniElectricidad.Compartido.DTOs.Servicios;

public record ServicioActivoDto
{
    public int Id { get; set; }
    public required string Titulo { get; init; }
    public string? Descripcion { get; init; }
    public string? ImagenUrl { get; init; }
}