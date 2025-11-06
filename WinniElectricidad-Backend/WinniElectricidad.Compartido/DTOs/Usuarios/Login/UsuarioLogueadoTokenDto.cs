namespace WinniElectricidad.Compartido.DTOs.Usuarios.Login;

public class UsuarioLogueadoTokenDto : UsuarioLogueadoDto
{
    public required string Token { get; set; }
}