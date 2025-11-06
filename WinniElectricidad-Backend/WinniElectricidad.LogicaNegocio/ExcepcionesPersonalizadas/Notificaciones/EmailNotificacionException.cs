namespace WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Notificaciones;

public class EmailNotificacionException : Exception
{
    public EmailNotificacionException()
    {
    }

    public EmailNotificacionException(string? message) : base(message)
    {
    }

    public EmailNotificacionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}