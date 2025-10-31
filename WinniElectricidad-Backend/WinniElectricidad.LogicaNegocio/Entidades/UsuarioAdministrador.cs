namespace WinniElectricidad.LogicaNegocio.Entidades;

public class UsuarioAdministrador : UsuarioBase
{
    public IEnumerable<Notificacion> NotificacionesEnviadas  { get; set; }
    
    public UsuarioAdministrador(string nombreCompleto, string email, string telefono) : base(nombreCompleto, email, telefono)
    {
        NotificacionesEnviadas =  new List<Notificacion>();
    }
}