using System.ComponentModel.DataAnnotations;

namespace WinniElectricidad.LogicaNegocio.Entidades;

public class Direccion
{
    #region Propiedades
    [Key]
    public int IdDireccion { get; set; }
    public string Calle { get; set; }
    public string Esquina { get; set; }
    public string? Numero { get; set; }
    public string? Apto { get; set; }
    #endregion
    
    #region EF
    public int IdUsuario {  get; set; }
    #endregion

    public Direccion(string calle, string esquina, string? numero, string? apto)
    {
        Calle = calle;
        Esquina = esquina;
    }
}