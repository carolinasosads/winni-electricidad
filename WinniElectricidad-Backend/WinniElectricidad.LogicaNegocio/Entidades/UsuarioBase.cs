using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.LogicaNegocio.Entidades;

public abstract class UsuarioBase
{
    #region Propiedades
    [Key]
    public int IdUsuario { get; set; } // TODO: ver como hacer que sea autoincremental
    public required string PasswordHash { get; set; }
    public required string NombreCompleto { get; set; }
    public required string Email { get; set; }
    public required string Telefono { get; set; }
    public IEnumerable<Notificacion> NotificacionesRecibidas  { get; set; } = new List<Notificacion>();
    public abstract string Rol {  get; }
    #endregion

    public UsuarioBase(){}
    public UsuarioBase(string nombreCompleto, string passwordHash, string email, string telefono)
    {
        NombreCompleto = nombreCompleto;
        PasswordHash = passwordHash;
        Email = email;
        Telefono = telefono;
    }
}