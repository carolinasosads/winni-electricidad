namespace WinniElectricidad.LogicaNegocio.Entidades;

public class UsuarioCliente : UsuarioBase
{
    #region Propiedades

    public IEnumerable<Direccion>  Direcciones { get; set; } = new List<Direccion>();
    public IEnumerable<Reserva> Reservas { get; set; } = new List<Reserva>();
    public IEnumerable<Pago>  Pagos { get; set; } = new List<Pago>();
    //public IEnumerable<Presupuesto>  Presupuestos { get; set; } = new List<Presupuesto>();

    #endregion
    
    public UsuarioCliente(){}
    public UsuarioCliente(string nombreCompleto, string passwordHash, string email, string telefono, IEnumerable<Direccion> direcciones) : base(nombreCompleto, passwordHash, email, telefono)
    {
        Direcciones = direcciones;
        Reservas = new List<Reserva>();
        Pagos = new List<Pago>();
    }
}
