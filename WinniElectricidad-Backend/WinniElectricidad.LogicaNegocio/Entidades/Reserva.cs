using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Reserva
{
    #region  Propiedades
    [Key]
    public int IdReserva { get; set; }
    public required ICollection<Servicio> Servicios { get; set; }
    #endregion

    public Reserva() {}
    
    public Reserva(ICollection<Servicio> servicios)
    {
        Servicios = servicios;
    }
}