namespace WinniElectricidad.Compartido.DTOs.Usuarios.Login;

public record UsuarioLogueadoTokenDto : UsuarioLogueadoDto
{
    public required string Token { get; init; }
}