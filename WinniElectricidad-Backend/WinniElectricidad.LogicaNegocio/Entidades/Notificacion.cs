using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Notificacion
{
    #region  Propiedades
    [Key]
    public int IdNotificacion { get; set; } // TODO: ver como hacer que sea autoincremental
    #endregion
}