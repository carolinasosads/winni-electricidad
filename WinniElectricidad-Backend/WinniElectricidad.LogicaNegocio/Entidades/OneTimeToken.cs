namespace WinniElectricidad.LogicaNegocio.Entidades;

public enum OneTimeTokenTipo { ReseteoDeContrasena }

public class OneTimeToken
{
    #region  Propiedades
    public int Id { get; set; }
    public OneTimeTokenTipo Tipo { get; set; }
    public required string TokenHash { get; set; }
    public DateTimeOffset ExpiraEl { get; set; }
    public bool Usado { get; set; }
    public DateTimeOffset CreadoEl { get; set; }
    #endregion
    #region EF
    public int IdUsuario { get; set; }
    public UsuarioBase Usuario { get; set; } = null!;
    #endregion
    
    public OneTimeToken()
    {
        CreadoEl = DateTimeOffset.UtcNow;
        ExpiraEl = DateTimeOffset.UtcNow.AddMinutes(30);
    }
}