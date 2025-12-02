using Resend;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Notificaciones;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Notificacion;

public class EnviarEmail : IEnviarEmail
{
    private readonly IResend _resend;
    
    public EnviarEmail(IResend resend)
    {
        _resend = resend;
    }
    
    public async Task Ejecutar(string destinatario, string asunto, string cuerpo, CancellationToken ct = default)
    {
        var emailMessage = new EmailMessage
        {
            From = "Winni Electricidad <no-replay@no-replay.winnielectricidad.tech>",
            To = {destinatario},  
            Subject = asunto,
            HtmlBody = cuerpo
        };

        try
        {
            await _resend.EmailSendAsync(emailMessage, ct);
        }
        catch (Exception ex)
        {
            throw new EmailNotificacionException("Error enviando correo de notificación.", ex);
        }
    }
}