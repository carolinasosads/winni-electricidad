using WinniElectricidad.Compartido.DTOs.Presupuesto;

namespace WinniElectricidad.Compartido.DTOs.Mappers;

public static class PresupuestoMapper
{
    public static LogicaNegocio.Entidades.Presupuesto MapearDtoAPresupuesto(PresupuestoCrearDto dto, int idReserva, int idUsuarioCliente)
    {
        var notas = string.IsNullOrWhiteSpace(dto.NotasInternas)
            ? null
            : dto.NotasInternas.Trim();

        return new LogicaNegocio.Entidades.Presupuesto
        {
            IdReserva = idReserva,
            IdUsuario = idUsuarioCliente,
            Monto = dto.MontoTotal,
            MontoPagado = dto.MontoPagado,
            DescripcionTrabajo = dto.DescripcionTrabajo,
            Notas = notas,
            FechaPresupuesto = DateTime.UtcNow
        };
    }

    public static PresupuestoDto MapearAPresupuestoDto(LogicaNegocio.Entidades.Presupuesto presupuesto) 
    {
        return new PresupuestoDto
        {
            Id = presupuesto.Id,
            IdReserva = presupuesto.IdReserva,
            MontoTotal = presupuesto.Monto,
            MontoPagado = presupuesto.MontoPagado,
            DescripcionTrabajo = presupuesto.DescripcionTrabajo,
            FechaCreacion = presupuesto.FechaPresupuesto
        };
    }
}