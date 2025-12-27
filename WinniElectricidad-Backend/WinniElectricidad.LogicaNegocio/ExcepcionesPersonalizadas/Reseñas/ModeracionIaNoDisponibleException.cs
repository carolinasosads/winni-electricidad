namespace WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reseñas;

public class ModeracionIaNoDisponibleException : Exception
{
    public ModeracionIaNoDisponibleException()
    {
    }

    public ModeracionIaNoDisponibleException(string? message) : base(message)
    {
    }

    public ModeracionIaNoDisponibleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}