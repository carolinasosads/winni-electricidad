using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.LogicaNegocio.Entidades;

public class UsuarioBase
{
    #region Propiedades
    [Key]
    public int IdUsuario { get; set; } // TODO: ver como hacer que sea autoincremental
    public string PasswordHash { get; set; }
    public string NombreCompleto { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
    public IEnumerable<Notificacion> NotificacionesRecibidas  { get; set; }
    #endregion

    public UsuarioBase(string nombreCompleto, string passwordHash, string email, string telefono)
    {
        NombreCompleto = nombreCompleto;
        PasswordHash = passwordHash;
        Email = email;
        Telefono = telefono;
        NotificacionesRecibidas = new List<Notificacion>();
    }
}