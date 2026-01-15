using Resend;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Notificaciones;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Notificacion;

public class EnviarEmail : IEnviarEmail
{
    private readonly IResend _resend;
    private const string FooterHtml = @"
        <div style='border-top:1px solid #e0e0e0; margin-top:24px; padding-top:16px; font-size:12px; color:#777;'>
          <p style='margin:0;'>
            Winni Electricidad<br />
            Servicio técnico en electricidad, sanitaria, climatización y riego.
          </p>
          <p style='margin:8px 0 0 0;'>
            Este es un mensaje automático, por favor no responder este correo.
          </p>
        </div>";
    
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
    
    public string GetFooter()
    {
        return FooterHtml;
    }
}