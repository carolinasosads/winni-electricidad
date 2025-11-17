using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Reserva
{
    #region  Propiedades
    [Key]
    public int IdReserva { get; set; }
    
    public DateTime FechaReserva { get; set; }
    public EstadoReserva EstadoReserva { get; set; }
    public TipoServicioReserva TipoServicioReserva { get; set; }
    public string? Comentario { get; set; }
    
    [Required]
    public int IdUsuarioCliente { get; set; }

    [Required]
    public int IdDireccion { get; set; }
    public Direccion Direccion { get; set; }  = null!;

    public Presupuesto? Presupuesto { get; set; }

    public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
    #endregion

    public Reserva() {}
    
    public Reserva(DateTime fechaReserva,
        TipoServicioReserva tipo,
        int idUsuarioCliente,
        int idDireccion,
        ICollection<Servicio> servicios,
        string? comentario)
    {
        FechaReserva = fechaReserva;
        EstadoReserva = EstadoReserva.Pendiente;
        TipoServicioReserva = tipo;
        IdUsuarioCliente = idUsuarioCliente;
        IdDireccion = idDireccion;
        Servicios = servicios;
        Comentario = comentario;
        Validar();
    }

    private void Validar()
    {
        ValidarFechaReserva(FechaReserva);
        ValidarTipoServicioReserva(TipoServicioReserva);
        ValidarIds(IdDireccion, IdUsuarioCliente);
        ValidarServicios(Servicios);
    }

    private static void ValidarFechaReserva(DateTime fechaReserva)
    {
        var hoy = DateTime.Today;

        var minimo = hoy.AddDays(2);

        if (fechaReserva.Date < minimo)
        {
            throw new ArgumentException("Las reservas deben hacerse con al menos 48 horas de anticipación.");
        }

        if (fechaReserva.Date > hoy.AddDays(30))
        {
            throw new ArgumentException("No se pueden reservar turnos con más de 30 días de anticipación.");
        }
    }

    private static void ValidarTipoServicioReserva(TipoServicioReserva tipoServicioReserva)
    {
        if (!Enum.IsDefined(typeof(TipoServicioReserva), tipoServicioReserva)) 
        {
            throw new ArgumentException("Debe seleccionar un tipo correcto: Instalación o Mantenimiento.");
        }
    }

    private static void ValidarIds(int idDireccion, int idUsuarioCliente)
    {
        if (idDireccion <= 0)
        {
            throw new ArgumentException("Debe seleccionar una direccion válida.");
        }
        
        if (idUsuarioCliente <= 0)
        {
            throw new ArgumentException("Debe seleccionar un cliente válido.");
        }
    }

    private static void ValidarServicios(ICollection<Servicio> servicios)
    {
        if (servicios == null || servicios.Count == 0)
            throw new ArgumentException("La reserva debe contener al menos un servicio.");
    }
}