namespace WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;

public class UsuarioReservaDto
{
    public int IdUsuario { get; set; }
    public string Nombre { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefono { get; set; }
}