namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Pago
{
    #region Propiedades
    public int IdPago { get; set; }
    public double Monto { get; }
    public DateTime FechaHoraRealizado { get; }
    #endregion
    
    #region EF
    public int IdUsuario { get; set; }
    public UsuarioCliente Usuario { get; set; }
    #endregion

    public Pago(double monto, UsuarioCliente usuario)
    {
        Monto = monto;
        FechaHoraRealizado = DateTime.Now;
        Usuario = usuario;
    }
}