namespace WinniElectricidad.Compartido.DTOs.Servicios;

public record ServicioDto : ServicioActivoDto
{
    public bool Activo { get; init; }
}