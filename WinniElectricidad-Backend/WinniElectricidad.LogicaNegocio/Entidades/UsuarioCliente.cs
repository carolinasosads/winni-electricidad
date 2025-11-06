namespace WinniElectricidad.LogicaNegocio.Entidades;

public class UsuarioCliente : UsuarioBase
{
    #region Propiedades

    public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
   //public IEnumerable<Direccion>  Direcciones { get; set; } = new List<Direccion>();
    public IEnumerable<Reserva> Reservas { get; set; } = new List<Reserva>();
    public IEnumerable<Pago>  Pagos { get; set; } = new List<Pago>();
    //public IEnumerable<Presupuesto>  Presupuestos { get; set; } = new List<Presupuesto>();
    public override string Rol => "Cliente";

    #endregion
    public UsuarioCliente() { }
    
    public UsuarioCliente(string nombreCompleto, string passwordHash, string email, string telefono, ICollection<Direccion> direcciones) : base(nombreCompleto, passwordHash, email, telefono)
    {
        Direcciones = direcciones;
        Reservas = new List<Reserva>();
        Pagos = new List<Pago>();
        Validar();
    }

    private void Validar()
    {
        if (Direcciones is null)
        {
            throw new Exception("Ingrese una dirección valida");
        }
    //No me valida las direcciones extra
    }
}
