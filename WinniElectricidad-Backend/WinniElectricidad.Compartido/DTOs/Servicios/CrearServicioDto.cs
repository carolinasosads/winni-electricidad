using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.Compartido.DTOs.Servicios;

public class CrearServicioDto
{
    [Required]
    public required string Titulo { get; init; }
    
    [Required]
    public string Descripcion { get; init; }
    [Required]
    public string Icono { get; init; }
}