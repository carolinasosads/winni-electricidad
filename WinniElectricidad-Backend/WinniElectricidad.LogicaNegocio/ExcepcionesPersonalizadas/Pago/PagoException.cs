namespace WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Pago;

public class PagoException : Exception
{
    public PagoException()
    {
    }

    public PagoException(string? message) : base(message)
    {
    }

    public PagoException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}