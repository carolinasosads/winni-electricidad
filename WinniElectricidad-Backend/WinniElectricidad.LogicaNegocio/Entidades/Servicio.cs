namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Servicio
{
    #region  Propiedades
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string? Descripcion { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<Reseña>  Reseñas { get; set; } = new List<Reseña>();
    #endregion  

    public Servicio() {}
    
    public Servicio(string titulo, string? descripcion, string? imagenUrl)
    {
        Titulo = titulo;
        Descripcion = descripcion;
        ImagenUrl = imagenUrl;
        Reseñas = new List<Reseña>();
        Validar();
    }

    private void Validar()
    {
        ValidarTitulo(Titulo);
        ValidarDescripcion(Descripcion);
    }
    
    private void ValidarTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo)) {
            throw new ArgumentException("Ingresa un título válido.", nameof(titulo));
        }
    }

    private void ValidarDescripcion(string? descripcion)
    {
        if (descripcion is not null && descripcion.Length <= 15) {
            throw new ArgumentException("Ingresa una descripción más detallada.", nameof(descripcion));
        }
    }
    
    public void Desactivar()
    {
        if (!Activo)
            throw new InvalidOperationException("El servicio ya está desactivado.");
        Activo = false;
    }
    
    public void Activar()
    {
        if (Activo)
            throw new InvalidOperationException("El servicio ya está activo.");
        Activo = true;
    }
}