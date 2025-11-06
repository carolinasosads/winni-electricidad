namespace WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Tokens;

public class OneTimeTokenException : Exception
{
    public OneTimeTokenException()
    {
    }

    public OneTimeTokenException(string? message) : base(message)
    {
    }

    public OneTimeTokenException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}