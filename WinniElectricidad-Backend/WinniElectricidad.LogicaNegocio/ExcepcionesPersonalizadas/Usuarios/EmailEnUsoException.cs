namespace WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Usuarios;

public class EmailEnUsoException : Exception
{
    public EmailEnUsoException()
    { }
    
    public EmailEnUsoException(string?message) : base(message)
    { }
    
    public EmailEnUsoException(string? message, Exception? innerException) : base(message, innerException)
    { }
}