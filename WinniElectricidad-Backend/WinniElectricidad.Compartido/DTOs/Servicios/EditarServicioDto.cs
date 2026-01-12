namespace WinniElectricidad.Compartido.DTOs.Servicios;

public class EditarServicioDto
{
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public List<string> ImagenesExistentes { get; set; } = [];
    public string? UrlPrincipalExistente { get; set; }
    public int? IndexPrincipalNueva { get; set; }
}