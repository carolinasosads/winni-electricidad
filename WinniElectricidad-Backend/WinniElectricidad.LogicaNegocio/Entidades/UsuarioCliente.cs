namespace WinniElectricidad.LogicaNegocio.Entidades;

public class UsuarioCliente : UsuarioBase
{
    #region Propiedades

    public IEnumerable<Direccion>  Direcciones { get; set; }
    public IEnumerable<Reserva> Reservas { get; set; }
    public IEnumerable<Pago>  Pagos { get; set; }
    public IEnumerable<Presupuesto>  Presupuestos { get; set; }

    #endregion
    
    public UsuarioCliente(string nombreCompleto, string email, string telefono, IEnumerable<Direccion> direcciones) : base(nombreCompleto, email, telefono)
    {
        Direcciones = direcciones;
        Reservas = new List<Reserva>();
        Pagos = new List<Pago>();
        Presupuestos = new List<Presupuesto>();
    }
}
