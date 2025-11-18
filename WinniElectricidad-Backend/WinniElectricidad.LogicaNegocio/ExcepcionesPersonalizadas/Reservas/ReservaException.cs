namespace WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reservas;

public class ReservaException : Exception
{
    public ReservaException()
    {
    }

    public ReservaException(string? message) : base(message)
    {
    }

    public ReservaException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}