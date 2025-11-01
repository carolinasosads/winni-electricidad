using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Pago
{
    #region Propiedades
    [Key]
    public int IdPago { get; set; }
    public double Monto { get; set; }
    public DateTime FechaHoraRealizado { get; }
    #endregion
    
    #region EF
    public int IdUsuario { get; set; }
    public required UsuarioCliente Usuario { get; set; }
    #endregion

    public Pago(){}
    public Pago(double monto, UsuarioCliente usuario)
    {
        Monto = monto;
        FechaHoraRealizado = DateTime.Now;
        Usuario = usuario;
    }
}