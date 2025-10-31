namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Pago
{
    #region Propiedades
    public int IdPago { get; set; }
    public double Monto { get; }
    public DateTime FechaHoraRealizado { get; }
    #endregion

    public Pago(double monto)
    {
        Monto = monto;
        FechaHoraRealizado = DateTime.Now;
    }
}