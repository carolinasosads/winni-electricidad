namespace WinniElectricidad.Compartido.DTOs.Usuarios.Login;

public record UsuarioLogueadoDto
{
    public int Id { get; init; }
    public required string Email { get; init; }
    public required string Rol { get; init; }
}