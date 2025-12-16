namespace WinniElectricidad.LogicaNegocio.Entidades;

public class UsuarioCliente : UsuarioBase
{
    #region Propiedades

    public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<Pago>  Pagos { get; set; } = new List<Pago>();
    public ICollection<Presupuesto>  Presupuestos { get; set; } = new List<Presupuesto>();
    public ICollection<Reseña>  Reseñas { get; set; } = new List<Reseña>();
    public override string Rol => "Cliente";

    #endregion
    public UsuarioCliente() { }
    
    public UsuarioCliente(string nombreCompleto, string passwordHash, string email, string telefono, ICollection<Direccion> direcciones) : base(nombreCompleto, passwordHash, email, telefono)
    {
        Direcciones = direcciones;
        Reservas = new List<Reserva>();
        Pagos = new List<Pago>();
        Presupuestos = new List<Presupuesto>();
        Reseñas = new List<Reseña>();
        Validar();
    }

    private void Validar()
    {
        if (Direcciones is null)
        {
            throw new Exception("Ingrese una dirección valida");
        }
        //TODO validar las direcciones extra
    }
}
