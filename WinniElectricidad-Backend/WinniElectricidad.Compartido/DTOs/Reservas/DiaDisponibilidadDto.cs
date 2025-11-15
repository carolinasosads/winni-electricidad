namespace WinniElectricidad.Compartido.DTOs.Reservas;

/// <summary>
/// Representa un día con su listado de horarios disponibles o no disponibles.
/// </summary>
public record DiaDisponibilidadDto
{
    public DateTime Fecha { get; init; }
    public IEnumerable<HoraDto> Horas { get; init; }
}