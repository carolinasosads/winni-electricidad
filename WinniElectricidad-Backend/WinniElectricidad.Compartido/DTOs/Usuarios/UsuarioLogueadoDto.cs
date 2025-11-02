namespace WinniElectricidad.Compartido.DTOs.Usuarios;

public class UsuarioLogueadoDto
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Rol { get; set; }
}