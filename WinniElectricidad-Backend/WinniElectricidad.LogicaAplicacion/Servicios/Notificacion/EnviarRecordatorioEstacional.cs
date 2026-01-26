using System.ComponentModel.DataAnnotations;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Notificacion;

public class EnviarRecordatorioEstacional : IEnviarRecordatorioEstacional
{
    private readonly IEnviarEmail _enviarEmail;

    public EnviarRecordatorioEstacional(IEnviarEmail enviarEmail)
    {
        _enviarEmail = enviarEmail;
    }
    
    public async Task Ejecutar(string tituloServicio, string texto, List<string> emailClientesParaEnviar, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(tituloServicio))
            throw new ArgumentException("No se envió el título del servicio.");

        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("No se agregó un texto para el email.");

        if (emailClientesParaEnviar == null || emailClientesParaEnviar.Count == 0)
            throw new ArgumentException("Debes seleccionar por lo menos un destinatario.");

        if (emailClientesParaEnviar.Any(e => string.IsNullOrWhiteSpace(e)))
            throw new ArgumentException("La lista de destinatarios contiene emails inválidos.");
        
        var emailValidator = new EmailAddressAttribute();
        
        if (emailClientesParaEnviar.Any(e => !emailValidator.IsValid(e)))
        {
            throw new ArgumentException(
                "La lista de destinatarios contiene emails con formato inválido."
            );
        }
        
        var cuerpoHtml = $@"
            <div style='font-family: Arial, sans-serif; background-color:#f4f6f8; padding:16px; box-sizing:border-box;'>
                <div style='
                    max-width:600px;
                    margin:0 auto;
                    background-color:#ffffff;
                    border-radius:8px;
                    overflow:hidden;
                    box-shadow:0 2px 6px rgba(0,0,0,0.05);
                '>
                    <!-- Header -->
                    <div style='background-color:#1f3a5f; color:#ffffff; padding:16px 24px;'>
                        <h2 style='margin:0; font-size:20px;'>Winni Electricidad</h2>
                        <p style='margin:4px 0 0; font-size:13px; opacity:0.9;'>
                            Servicios técnicos y mantenimiento
                        </p>
                    </div>

                    <!-- Body -->
                    <div style='padding:24px; color:#333;'>
                        <h3 style='margin-top:0; color:#1f3a5f;'>
                            Mantenimiento recomendado antes de la temporada - {tituloServicio}
                        </h3>

                        <p>Hola 👋,</p>

                        <p style='line-height:1.5; font-size:15px;'>
                            {texto}
                        </p>

                        <!-- Bloque informativo -->
                        <div style='
                            background:#f5f7fa;
                            padding:12px;
                            border-left:4px solid #1f3a5f;
                            margin:24px 0;
                            font-size:14px;
                        '>
                            Coordinar estos trabajos con anticipación ayuda a evitar esperas y a asegurar
                            disponibilidad durante los momentos de mayor demanda.
                        </div>

                        <p>
                            Si querés coordinar una revisión o tenés alguna consulta, quedamos a las órdenes.
                        </p>

                        <p>
                            Saludos,<br/>
                            <strong>Winni Electricidad</strong>
                        </p>

                        {_enviarEmail.GetFooterRecordatorioEstacional()}
                    </div>
                </div>
            </div>";
        
        await _enviarEmail.EjecutarMultiple(
            emailClientesParaEnviar,
            $"Winni Electricidad - Mantenimiento recomendado antes de la temporada",
            cuerpoHtml,
            ct
        );
    }
}