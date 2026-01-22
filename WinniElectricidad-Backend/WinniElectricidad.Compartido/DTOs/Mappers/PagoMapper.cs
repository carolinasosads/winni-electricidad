using WinniElectricidad.Compartido.DTOs.Pago;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.DTOs.Mappers;

public static class PagoMapper
{
    public static LogicaNegocio.Entidades.Pago MapearAPago(PagoCrearDto dto, int idPresupuesto, int idUsuario)
    {
        return new LogicaNegocio.Entidades.Pago(dto.Monto, idPresupuesto, idUsuario, EstadoPago.Confirmado);
    }

    public static LogicaNegocio.Entidades.Pago MapearAPagoPendiente(PagoPendienteDto dto, int idUsuario)
    {
        return new LogicaNegocio.Entidades.Pago(dto.Monto, dto.IdPresupuesto, idUsuario, EstadoPago.Pendiente);
    }

    public static PagoPendienteDevueltoDto MapearPagoAPagoDevuelto(string idPreference, LogicaNegocio.Entidades.Pago pago)
    {
        return new PagoPendienteDevueltoDto
        {
            IdPreference = idPreference,
            IdPagoPendiente = pago.IdPago
        };
    }
}