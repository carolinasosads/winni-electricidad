namespace WinniElectricidad.Compartido.DTOs.Usuarios.Busqueda;

public class UsuarioBusquedaDto
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
}