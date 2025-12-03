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

    public RecuperarContrasena(IRepositorioUsuario repositorioUsuario, IRepositorioOneTimeToken repositorioOneTimeToken, IServicioOneTimeToken servicioOneTimeToken, IEnviarEmail enviarEmail)
    {
        _repositorioUsuario = repositorioUsuario;
        _repositorioOneTimeToken = repositorioOneTimeToken;
        _servicioOneTimeToken = servicioOneTimeToken;
        _enviarEmail = enviarEmail;
    }
    
    public async Task EnviarCorreoRecuperacion(string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return;

        var usuarioEncontrado = await _repositorioUsuario.FindbyEmail(email, ct);
        if (usuarioEncontrado is null) return;
        
        var activo = await _repositorioOneTimeToken.GetActiveByUser(usuarioEncontrado.IdUsuario, ct);
        //if (activo is not null) return;
        
        var (tokenPlain, tokenHash) = _servicioOneTimeToken.Create();

        await _repositorioOneTimeToken.Add(new OneTimeToken
        {
            Tipo = OneTimeTokenTipo.ReseteoDeContrasena,
            TokenHash =  tokenHash,
            Usado = false,
            IdUsuario = usuarioEncontrado.IdUsuario,
        }, ct);
    
        var urlRecuperacionContrasena = $"https://icy-flower-09db15f0f.3.azurestaticapps.net/reset-password?token={Uri.EscapeDataString(tokenPlain)}";

        var cuerpo = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Recuperación de contraseña</h2>
                <p>Hola {usuarioEncontrado.NombreCompleto},</p>
                <p>Hacé clic en el siguiente enlace para restablecer tu contraseña:</p>
                <p>
                    <a href=""{urlRecuperacionContrasena}"">Restablecer contraseña</a>
                </p>
                <p>Ten en cuenta que este link es válido únicamente por los siguientes 30 minutos.</p>
                <p>Si no solicitaste este cambio, podés ignorar este mensaje.</p>
                <hr />
                <p style='font-size:12px; color:#888;'>Este mensaje fue enviado por Winni Electricidad mediante Resend.</p>
            </div>";

        await _enviarEmail.Ejecutar(email, "Recuperación de contraseña - Winni Electricidad", cuerpo, ct);
    }

    public async Task ResetearContrasena(string password, string tokenPlain, CancellationToken ct = default)
    {
        if (password is null || tokenPlain is null) return;

        var tokenHash = _servicioOneTimeToken.Hash(tokenPlain);

        OneTimeToken? tokenActivo = await _repositorioOneTimeToken.GetActiveByHash(tokenHash, ct);

        if (tokenActivo is null) throw new OneTimeTokenException("No existen tokens activos con el string proporcionado o el token ya expiró.");
        // TODO: si es valido, hashear la contrasena 
        await _repositorioUsuario.ChangePassword(tokenActivo.IdUsuario, password, ct);
        
        await _repositorioOneTimeToken.MarkUsed(tokenHash, ct);
    }
}
