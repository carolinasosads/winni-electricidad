using WinniElectricidad.Compartido.DTOs.Pago;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Pago;

public interface ICrearPreferenciaPago
{
    Task<PagoPendienteDevueltoDto> Ejecutar(PagoPendienteDto pagoPendienteDto, int idUsuario,  CancellationToken ct = default);
}