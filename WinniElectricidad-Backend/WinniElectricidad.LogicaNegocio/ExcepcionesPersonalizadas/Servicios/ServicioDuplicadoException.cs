namespace WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Servicios;

public class ServicioDuplicadoException: Exception
{
    public ServicioDuplicadoException()
    {
    }

    public ServicioDuplicadoException(string? message) : base(message)
    {
    }

    public ServicioDuplicadoException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}