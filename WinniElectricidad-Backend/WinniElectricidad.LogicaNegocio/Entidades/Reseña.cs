namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Reseña
{
    #region  Propiedades
    public int Id { get; set; }
    public string Descripcion { get; set; }
    public int Calificacion { get; set; }
    public DateTime FechaPublicacion  { get; set; }
    public string? ImagenUrl { get; set; }
    public int IdUsuario { get; set; }
    public int IdServicio { get; set; }
    public EstadoReseña Estado { get; set; }
    #endregion

    public Reseña(){}
    public Reseña(string descripcion,int calificacion, int idUsuario, int idServicio, string? imagen)
    {
        Descripcion = descripcion;
        Calificacion = calificacion;
        FechaPublicacion  = DateTime.Now;
        IdUsuario = idUsuario;
        IdServicio = idServicio;
        ImagenUrl = imagen;
        Estado = EstadoReseña.Aprobada; //TODO: cambiar a pendiente cuando este la moderacion mediante IA
        Validar();
    }

    private void Validar()
    {
        ValidarDescripcion(Descripcion);
        ValidarCalificacion(Calificacion);
        ValidarIds(IdServicio, IdUsuario);
    }
    
    private static void ValidarDescripcion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion)) {
            throw new ArgumentException("Ingrese una descripción de reseña válida.", nameof(descripcion));
        }
    }
    
    private static void ValidarCalificacion(int calificacion)
    {
        if (calificacion is <= 0 or > 5)
        {
            throw new ArgumentException("Debes colocar una calificación mayor o igual a 1 y menor o igual a 5.");
        }
    }
    
    private static void ValidarIds(int idServicio, int idUsuarioCliente)
    {
        if (idServicio <= 0)
        {
            throw new ArgumentException("Debes seleccionar una servicio válido.");
        }

        if (idUsuarioCliente <= 0)
        {
            throw new ArgumentException("Debes seleccionar un cliente válido.");
        }
    }
}