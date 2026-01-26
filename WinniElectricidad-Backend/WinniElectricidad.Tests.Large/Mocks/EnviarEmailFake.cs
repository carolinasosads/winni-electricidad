using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;

namespace WinniElectricidad.Tests.Large.Mocks;

public class EnviarEmailFake : IEnviarEmail
{
    public Task Ejecutar(
        string email,
        string asunto,
        string cuerpoHtml,
        CancellationToken ct)
    {
        return Task.CompletedTask;
    }

    public string GetFooter()
    {
        return "<footer>Fake footer</footer>";
    }

    public string GetFooterRecordatorioEstacional()
    {
        return "<footer>Fake footer</footer>";
    }
}