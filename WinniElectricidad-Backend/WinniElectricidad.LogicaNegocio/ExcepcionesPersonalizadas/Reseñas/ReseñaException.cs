namespace WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reseñas;

public class ReseñaException : Exception
{
    public ReseñaException()
    {
    }

    public ReseñaException(string? message) : base(message)
    {
    }

    public ReseñaException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}