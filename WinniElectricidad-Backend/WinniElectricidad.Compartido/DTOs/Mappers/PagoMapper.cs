using WinniElectricidad.Compartido.DTOs.Pago;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.DTOs.Mappers;

public static class PagoMapper
{
    public static LogicaNegocio.Entidades.Pago MapearAPago(PagoCrearDto dto, int idPresupuesto, int idUsuario)
    {
        return new LogicaNegocio.Entidades.Pago(dto.Monto, idPresupuesto, idUsuario);
    }
}