using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Reserva
{
    #region  Propiedades
    [Key]
    public int IdReserva { get; set; } // TODO: ver como hacer que sea autoincremental
    #endregion
}