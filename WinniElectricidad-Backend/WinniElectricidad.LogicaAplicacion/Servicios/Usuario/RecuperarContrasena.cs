using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Tokens;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class RecuperarContrasena : IRecuperarContrasena
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IRepositorioOneTimeToken _repositorioOneTimeToken;
    private readonly IServicioOneTimeToken  _servicioOneTimeToken;
    private readonly IEnviarEmail _enviarEmail;
    private readonly IServicioHash _servicioHash;

    public RecuperarContrasena(IRepositorioUsuario repositorioUsuario, IRepositorioOneTimeToken repositorioOneTimeToken, IServicioOneTimeToken servicioOneTimeToken, IEnviarEmail enviarEmail, IServicioHash servicioHash)
    {
        _repositorioUsuario = repositorioUsuario;
        _repositorioOneTimeToken = repositorioOneTimeToken;
        _servicioOneTimeToken = servicioOneTimeToken;
        _enviarEmail = enviarEmail;
        _servicioHash = servicioHash;
    }
    
    public async Task EnviarCorreoRecuperacion(string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return;

        var usuarioEncontrado = await _repositorioUsuario.FindbyEmail(email, ct);
        if (usuarioEncontrado is null) return;
        
        var activo = await _repositorioOneTimeToken.GetActiveByUser(usuarioEncontrado.IdUsuario, ct);
        if (activo is not null) return;
        
        var (tokenPlain, tokenHash) = _servicioOneTimeToken.Create();

        await _repositorioOneTimeToken.Add(new OneTimeToken
        {
            Tipo = OneTimeTokenTipo.ReseteoDeContrasena,
            TokenHash =  tokenHash,
            Usado = false,
            IdUsuario = usuarioEncontrado.IdUsuario,
        }, ct);
    
        var urlRecuperacionContrasena = $"https://icy-flower-09db15f0f.3.azurestaticapps.net/reset-password?token={Uri.EscapeDataString(tokenPlain)}";

        var footer = _enviarEmail.GetFooter();

        var cuerpo = $@"
            <div style=""font-family: Arial, sans-serif; background-color:#f4f6f8; padding:24px;"">
              <div style=""max-width:600px; margin:0 auto; background-color:#ffffff; border-radius:8px; overflow:hidden;"">

                <!-- Header -->
                <div style=""background-color:#1f3a5f; color:#ffffff; padding:16px 24px;"">
                  <h2 style=""margin:0; font-size:20px;"">Winni Electricidad</h2>
                </div>

                <!-- Body -->
                <div style=""padding:24px; color:#333333;"">
                  <h3 style=""margin-top:0; color:#1f3a5f;"">
                    Recuperación de contraseña
                  </h3>

                  <p style=""margin:0 0 12px 0;"">
                    Hola <strong>{usuarioEncontrado.NombreCompleto} 👋🏽</strong>,
                  </p>

                  <p style=""margin:0 0 16px 0;"">
                    Recibimos una solicitud para restablecer tu contraseña.
                    Para continuar, hacé clic en el siguiente botón:
                  </p>

                  <p style=""margin:24px 0; text-align:center;"">
                    <a
                      href=""{urlRecuperacionContrasena}""
                      style=""background-color:#1f3a5f; color:#ffffff; padding:12px 20px;
                             text-decoration:none; border-radius:6px; font-weight:bold; display:inline-block;"">
                      Restablecer contraseña
                    </a>
                  </p>

                  <p style=""margin:0 0 12px 0;"">
                    Este enlace es válido únicamente por los próximos <strong>30 minutos</strong>.
                  </p>

                  <p style=""margin:0;"">
                    Si no solicitaste este cambio, podés ignorar este mensaje.
                  </p>

                  {footer}
                </div>

              </div>
            </div>";

        await _enviarEmail.Ejecutar(email, "Winni Electricidad - Recuperación de contraseña", cuerpo, ct);
    }

    public async Task ResetearContrasena(string password, string tokenPlain, CancellationToken ct = default)
    {
        if (password is null || tokenPlain is null) return;

        var tokenHash = _servicioOneTimeToken.Hash(tokenPlain);

        OneTimeToken? tokenActivo = await _repositorioOneTimeToken.GetActiveByHash(tokenHash, ct);

        if (tokenActivo is null) throw new OneTimeTokenException("No existen tokens activos con el string proporcionado o el token ya expiró.");
        
        if (password.Length < 6)
            throw new ArgumentException("La contraseña debe tener al menos 6 dígitos.");
        
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Debes ingresar una contraseña.");
        
        var passwordHash = _servicioHash.Hash(password);
        
        await _repositorioUsuario.ChangePassword(tokenActivo.IdUsuario, passwordHash, ct);
        
        await _repositorioOneTimeToken.MarkUsed(tokenHash, ct);
    }
}
