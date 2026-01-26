namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Notificacion;

public interface IEnviarRecordatorioEstacional
{
    Task Ejecutar(string tituloServicio, string texto, List<string> emailClientesParaEnviar, CancellationToken ct);
}