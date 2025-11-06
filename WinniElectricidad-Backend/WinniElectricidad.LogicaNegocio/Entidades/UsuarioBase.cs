using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.LogicaNegocio.Entidades;

public abstract class UsuarioBase
{
    #region Propiedades
    [Key]
    public int IdUsuario { get; set; } // TODO: ver como hacer que sea autoincremental
    public string PasswordHash { get; set; }
    public string NombreCompleto { get; set; }
    public string Email { get; set; }
    public string Telefono { get; set; }
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
        Validar();
    }

    private void Validar()
    {
        ValidarNombreCompleto(NombreCompleto);
        ValidarPassword(PasswordHash);
        ValidarEmail(Email);
        ValidarTelefono(Telefono);
    }
    
    private void ValidarNombreCompleto(string nombreCompleto)
    {
        if (string.IsNullOrEmpty(nombreCompleto.Trim()))
        {
            throw new Exception("Ingrese nombre completo");
        }
    }
    
    private void ValidarPassword(string pass)
    {
        if (string.IsNullOrEmpty(pass.Trim()) || pass.Length < 6)
        {
            throw new Exception("Ingrese una contraseña mayor a 6 dígitos");
        }
    }
    
    private void ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new Exception("El email es obligatorio.");
        }
        email = email.Trim();
        if (!email.Contains("@") || !email.Contains(".")) throw new Exception("El email no es válido.");
        var arrobaIndex = email.IndexOf('@');
        var puntoIndex  = email.LastIndexOf('.');

        if (puntoIndex < arrobaIndex) throw new Exception("El email no es válido.");
    }
    private void ValidarTelefono(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
            throw new Exception("El teléfono es obligatorio.");

        telefono = telefono.Trim();

        if (telefono.Length < 7)
            throw new Exception("El teléfono debe tener al menos 7 dígitos.");

        //El.All es como el char
        if (!telefono.All(c => c >= '0' && c <= '9'))
            throw new Exception("El teléfono solo puede contener números.");
    } 
}