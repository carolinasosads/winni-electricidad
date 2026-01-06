using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Pago
{
    #region Propiedades
    [Key]
    public int IdPago { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Monto { get; set; }

    public DateTime FechaHoraRealizado { get; set; } = DateTime.UtcNow;
    #endregion

    #region EF
    public int IdUsuario { get; set; }
    public UsuarioCliente? Usuario { get; set; }   

    public int IdPresupuesto { get; set; }
    public Presupuesto Presupuesto { get; set; } = null!;
    #endregion

    public Pago() { }

    public Pago(decimal monto, int idPresupuesto, int idUsuario)
    {
        Monto = monto;
        FechaHoraRealizado = DateTime.UtcNow;
        IdPresupuesto = idPresupuesto;
        IdUsuario = idUsuario;
        Validar();
    }

    private void Validar()
    {
        ValidarMonto(Monto);
    }

    private void ValidarMonto(decimal monto)
    {
        if (monto <= 0m)
            throw new ArgumentException("El monto del pago debe ser mayor a 0.");
    }
}