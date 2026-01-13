namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Servicio
{
    #region  Propiedades
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    
    public ICollection<ServicioImagen> Imagenes { get; set; } = new List<ServicioImagen>();
    public ICollection<Reseña>  Reseñas { get; set; } = new List<Reseña>();
    #endregion  

    public Servicio() {}
    
    public Servicio(string titulo, string? descripcion, ICollection<ServicioImagen> imagenes)
    {
        Titulo = titulo;
        Descripcion = descripcion;
        Imagenes = imagenes;
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
            throw new ArgumentException("Ingresa un título válido.");
        }
    }

    private void ValidarDescripcion(string? descripcion)
    {
        if (descripcion is not null && descripcion.Length <= 15) {
            throw new ArgumentException("Ingresa una descripción más detallada.");
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

    public void Actualizar(string nuevoTitulo, string nuevaDescripcion, List<string> imagenesUrls, List<ServicioImagen>? nuevasImagenes, string? urlPrincipalFinal)
    {
        ValidarTitulo(nuevoTitulo);
        ValidarDescripcion(nuevaDescripcion);
        
        if (nuevoTitulo != Titulo)
        {
            Titulo  = nuevoTitulo;
        }
        
        if (nuevaDescripcion != Descripcion)
        {
            Descripcion  = nuevaDescripcion;
        }

        foreach (var imagen in Imagenes.ToList())
        {
            var imagenUrl = imagen.Url;
            if (!imagenesUrls.Contains(imagenUrl))
            {
                Imagenes.Remove(imagen);
            }
        }

        if (nuevasImagenes != null)
        {
            foreach (var imagen in nuevasImagenes)
            {
                Imagenes.Add(imagen);
            }
        }
        
        if (Imagenes.Count == 0)
        {
            throw new InvalidOperationException("El servicio debe tener al menos una imagen.");
        }

        ServicioImagen? principal = null;

        if (!string.IsNullOrWhiteSpace(urlPrincipalFinal))
        {
            principal = Imagenes.FirstOrDefault(i => i.Url == urlPrincipalFinal);
        }

        principal ??= Imagenes.FirstOrDefault(i => i.EsPrincipal);

        principal ??= Imagenes.First();

        foreach (var img in Imagenes)
        {
            if (img == principal) img.MarcarComoPrincipal();
            else img.DesmarcarPrincipal();
        }
    }
}