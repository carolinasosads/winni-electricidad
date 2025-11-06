namespace WinniElectricidad.Compartido.DTOs.Usuarios.RecuperacionContrasena;

public class UsuarioResetPasswordDto
{
    public required string Password { get; set; }
    public required string TokenPlain { get; set; }
}