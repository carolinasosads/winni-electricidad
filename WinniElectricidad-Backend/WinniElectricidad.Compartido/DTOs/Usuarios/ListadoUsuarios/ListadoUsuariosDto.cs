namespace WinniElectricidad.Compartido.DTOs.Usuarios.ListadoUsuarios;

public class ListadoUsuariosDto
{
    public int IdUsuario { get; set; }
    public string Email { get; set; } = "";
    public string NombreCompleto { get; set; } = "";
    public string Telefono { get; set; } = "";
}