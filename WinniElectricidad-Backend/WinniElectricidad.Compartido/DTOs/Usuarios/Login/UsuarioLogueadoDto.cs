namespace WinniElectricidad.Compartido.DTOs.Usuarios.Login;

public class UsuarioLogueadoDto
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Rol { get; set; }
}