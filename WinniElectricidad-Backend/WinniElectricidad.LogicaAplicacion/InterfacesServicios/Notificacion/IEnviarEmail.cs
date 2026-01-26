namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;

public interface IEnviarEmail
{
    Task Ejecutar(string destinatario, string asunto, string cuerpo, CancellationToken ct = default);
    string GetFooter();
    string GetFooterRecordatorioEstacional();
}