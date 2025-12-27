namespace WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reseñas;

public class ReseñaOfensivaException : Exception
{
    public ReseñaOfensivaException()
    {
    }

    public ReseñaOfensivaException(string? message) : base(message)
    {
    }

    public ReseñaOfensivaException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}