namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Settings
{
    public int Id { get; set; }

    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
    public int MinutosEntreTurnos { get; set; }

    public int DiasMinimos { get; set; }
    public int DiasMaximos { get; set; }
}