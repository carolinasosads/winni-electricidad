namespace WinniElectricidad.Compartido.DTOs.Reservas;

/// <summary>
/// Representa un horario específico dentro de un día,
/// indicando si se encuentra disponible o no.
/// </summary>
public record HoraDto
{
    public TimeOnly Hora { get; init; }
    public bool Disponible { get; init; }
}