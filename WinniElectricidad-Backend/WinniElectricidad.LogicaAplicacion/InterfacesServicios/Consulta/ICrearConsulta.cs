using WinniElectricidad.Compartido.DTOs.Consulta;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Consulta;

public interface ICrearConsulta
{
    Task Ejecutar(ConsultaCrearDto dto, CancellationToken ct = default);
}