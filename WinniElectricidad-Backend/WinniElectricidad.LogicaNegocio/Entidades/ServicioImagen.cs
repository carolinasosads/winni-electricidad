namespace WinniElectricidad.LogicaNegocio.Entidades;

public class ServicioImagen
{
    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public bool EsPrincipal { get; set; }
    
    public int ServicioId { get; set; }
    public Servicio Servicio { get; set; } = null!;
    
    public ServicioImagen(){}
    
    public ServicioImagen(Servicio servicio, string url, bool esPrincipal = false)
    {
        Servicio = servicio;
        Url = url;
        EsPrincipal = esPrincipal;
        Validar();
    }

    private void Validar()
    {
        if (string.IsNullOrWhiteSpace(Url))
            throw new ArgumentException("La URL de la imagen es obligatoria.");
        
        if (Servicio == null)
            throw new ArgumentException("El servicio de la imagen es obligatorio.");
    }

    public void MarcarComoPrincipal()
    {
        EsPrincipal = true;
    }
}

