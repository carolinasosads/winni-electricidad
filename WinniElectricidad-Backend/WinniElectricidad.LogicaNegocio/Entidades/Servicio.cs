namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Servicio
{
    #region  Propiedades
    public int Id { get; set; }
    public required string Titulo { get; set; }
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
        if (string.IsNullOrWhiteSpace(titulo))        {
            throw new Exception("Ingrese un título válido.");
        }
    }

    private void ValidarDescripcion(string? descripcion)
    {
        if (descripcion is not null && descripcion.Length <= 15)        {
            throw new Exception("Ingrese una descripción más detallada.");
        }
    }
}