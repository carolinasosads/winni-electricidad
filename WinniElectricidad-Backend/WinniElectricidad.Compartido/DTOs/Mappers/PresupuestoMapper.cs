using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.LogicaNegocio.Entidades;

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
            Notas = notas,
            FechaPresupuesto = DateTime.UtcNow,
            DescripcionTrabajo = dto.DescripcionTrabajo
        };
    }
    public static PresupuestoDto MapearAPresupuestoDto(LogicaNegocio.Entidades.Presupuesto presupuesto)
    {
        var montoPagadoCalculado = (presupuesto.Pagos?.Sum(p => p.Monto)) ?? 0m;

        return new PresupuestoDto
        {
            Id = presupuesto.Id,
            IdReserva = presupuesto.IdReserva,
            MontoTotal = presupuesto.Monto,
            MontoPagado = montoPagadoCalculado,
            Notas = presupuesto.Notas,
            FechaCreacion = presupuesto.FechaPresupuesto,
            DescripcionTrabajo = presupuesto.DescripcionTrabajo,
        };
    }
    
    public static PresupuestoConReservaDto MapearAPresupuestoConReservaDto(LogicaNegocio.Entidades.Presupuesto presupuesto)
    {
        if (presupuesto.Reserva == null)
            throw new InvalidOperationException("No se puede mapear PresupuestoConReservaDto si la reserva no está cargada.");

        var montoPagadoCalculado =
            (presupuesto.Pagos?.Sum(p => p.Monto)) ?? presupuesto.MontoPagado;

        return new PresupuestoConReservaDto
        {
            Id = presupuesto.Id,
            Reserva = ReservaMapper.MapearAReservaCreadaDto(presupuesto.Reserva),
            MontoTotal = presupuesto.Monto,
            MontoPagado = montoPagadoCalculado,
            DescripcionTrabajo = presupuesto.DescripcionTrabajo,
            FechaCreacion = presupuesto.FechaPresupuesto
        };
    }
}
