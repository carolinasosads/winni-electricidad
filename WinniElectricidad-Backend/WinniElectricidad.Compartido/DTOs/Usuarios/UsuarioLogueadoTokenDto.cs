namespace WinniElectricidad.Compartido.DTOs.Usuarios;

public class UsuarioLogueadoTokenDto : UsuarioLogueadoDto
{
    public required string Token { get; set; }
}