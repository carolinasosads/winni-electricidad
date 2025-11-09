namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Servicio
{
    #region  Propiedades
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string? Descripcion { get; set; }
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; } = true;
    #endregion  

    public Servicio() {}
    
    public Servicio(string titulo, string? descripcion, string? imagenUrl)
    {
        Titulo = titulo;
        Descripcion = descripcion;
        ImagenUrl = imagenUrl;
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
            throw new ArgumentException("Ingrese un título válido.", nameof(titulo));
        }
    }

    private void ValidarDescripcion(string? descripcion)
    {
        if (descripcion is not null && descripcion.Length <= 15) {
            throw new ArgumentException("Ingrese una descripción más detallada.", nameof(descripcion));
        }
    }
}