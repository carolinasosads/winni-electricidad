using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public int IdUsuarioCliente  { get; set; }     
    public UsuarioCliente? UsuarioCliente { get; set; } 
    #endregion

    public Direccion() { }
    public Direccion(string calle, string esquina, string? numero, string? apto)
    {
        Calle = calle;
        Esquina = esquina;
        Numero = numero;
        Apto = apto;
        Validar();
    }

    private void Validar()
    {
        ValidarCalle(Calle);
        ValidarEsquina(Esquina);
    }

    private void ValidarCalle(string calle)
    {
        if (string.IsNullOrEmpty(calle.Trim()))
        {
            throw new Exception("Ingrese una calle valida");
        }
    }

    public void ValidarEsquina(string esquina)
    {
        if (string.IsNullOrEmpty(esquina.Trim()))
        {
            throw new Exception("Ingrese una esquina valida");
        }
    }
}