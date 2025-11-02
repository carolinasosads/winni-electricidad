namespace WinniElectricidad.LogicaNegocio.Entidades;

public class UsuarioAdministrador : UsuarioBase
{
    public IEnumerable<Notificacion> NotificacionesEnviadas  { get; set; }
    public override string Rol => "Administrador";

    public UsuarioAdministrador(string nombreCompleto, string passwordHash, string email, string telefono) : base(nombreCompleto, passwordHash, email, telefono)
    {
        NotificacionesEnviadas =  new List<Notificacion>();
    }
}