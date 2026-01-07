namespace WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Servicios;

public class ServicioException : Exception
{
    public ServicioException()
    {
    }

    public ServicioException(string? message) : base(message)
    {
    }

    public ServicioException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}