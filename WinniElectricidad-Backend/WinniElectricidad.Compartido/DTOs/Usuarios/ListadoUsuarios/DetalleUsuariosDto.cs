using WinniElectricidad.Compartido.DTOs.Direcciones;
using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;

namespace WinniElectricidad.Compartido.DTOs.Usuarios.ListadoUsuarios;

public class DetalleUsuariosDto
{
    public string Email { get; set; } = "";
    public string NombreCompleto { get; set; } = "";
    public string Telefono { get; set; } = "";
    
    public List<DireccionDto> Direcciones { get; set; } = new();
    public List<UsuarioReservaDto> Reservas { get; set; } = new();
    public List<PresupuestoDto> Presupuestos { get; set; } = new();
}