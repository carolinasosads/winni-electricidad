using Resend;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Notificaciones;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Tokens;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios;

public class RecuperarContrasena : IRecuperarContrasena
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IRepositorioOneTimeToken _repositorioOneTimeToken;
    private readonly IResend _resend;
    private readonly IServicioOneTimeToken  _servicioOneTimeToken;

    public RecuperarContrasena(IRepositorioUsuario repositorioUsuario, IRepositorioOneTimeToken repositorioOneTimeToken, IResend resend, IServicioOneTimeToken servicioOneTimeToken)
    {
        _repositorioUsuario = repositorioUsuario;
        _repositorioOneTimeToken = repositorioOneTimeToken;
        _resend = resend;
        _servicioOneTimeToken = servicioOneTimeToken;
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
    
        var urlRecuperacionContrasena = $"http://localhost:3001/reset-password?token={Uri.EscapeDataString(tokenPlain)}";

        var mensaje = new EmailMessage
        {
            From = "Winni Electricidad <no-replay@no-replay.winnielectricidad.tech>",
            To = { email },
            Subject = "Recuperación de contraseña - Winni Electricidad",
            HtmlBody = $@"
            <div style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Recuperación de contraseña</h2>
                <p>Hola {usuarioEncontrado.NombreCompleto},</p>
                <p>Hacé clic en el siguiente enlace para restablecer tu contraseña:</p>
                <p>
                    {urlRecuperacionContrasena}
                </p>
                <p>Ten en cuenta que este link es válido únicamente por los siguientes 30 minutos.</p>
                <p>Si no solicitaste este cambio, podés ignorar este mensaje.</p>
                <hr />
                <p style='font-size:12px; color:#888;'>Este mensaje fue enviado por Winni Electricidad mediante Resend.</p>
            </div>"
        };

        try
        {
            await _resend.EmailSendAsync(mensaje, ct);
        }
        catch (Exception ex)
        {
            throw new EmailNotificacionException("Error enviando correo de recuperación.", ex);
        }
    }

    public async Task ResetearContrasena(string password, string tokenPlain, CancellationToken ct)
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