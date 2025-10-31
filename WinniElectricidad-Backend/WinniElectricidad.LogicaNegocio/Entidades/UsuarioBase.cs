namespace WinniElectricidad.LogicaNegocio.Entidades;

public class UsuarioBase
{
    #region Propiedades
    public int IdUsuario { get; set; } // ver como hacer que sea autoincremental
    public string NombreCompleto { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
    public IEnumerable<Notificacion> NotificacionesRecibidas  { get; set; }
    #endregion

    public UsuarioBase(string nombreCompleto, string email, string telefono)
    {
        NombreCompleto = nombreCompleto;
        Email = email;
        Telefono = telefono;
        NotificacionesRecibidas = new List<Notificacion>();
    }
}