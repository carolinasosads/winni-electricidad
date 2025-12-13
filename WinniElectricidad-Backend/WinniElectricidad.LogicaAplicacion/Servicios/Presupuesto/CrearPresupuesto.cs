using WinniElectricidad.Compartido.DTOs.Presupuesto;
using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Presupuesto;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Presupuesto;

public class CrearPresupuesto : ICrearPresupuesto
{
    private readonly IRepositorioReserva _repoReservas;
    private readonly IRepositorioPresupuesto _repoPresupuestos;

    public CrearPresupuesto(
        IRepositorioReserva repoReservas,
        IRepositorioPresupuesto repoPresupuestos)
    {
        _repoReservas = repoReservas;
        _repoPresupuestos = repoPresupuestos;
    }

    public async Task<PresupuestoDto> Ejecutar(int idReserva, PresupuestoCrearDto dto, CancellationToken ct = default)
    {
        if (idReserva <= 0)
            throw new ArgumentException("El id de la reserva es inválido.", nameof(idReserva));

        if (dto is null)
            throw new ArgumentException("Los datos del presupuesto son obligatorios.", nameof(dto));

        if (dto.MontoTotal <= 0)
            throw new ArgumentException("El monto total debe ser mayor a 0.", nameof(dto.MontoTotal));

        var reserva = await _repoReservas.FindById(idReserva, ct);
        if (reserva is null)
            throw new KeyNotFoundException($"No existe la reserva {idReserva}.");

        var idCliente = reserva.IdUsuarioCliente;

        var presupuesto = PresupuestoMapper.MapearDtoAPresupuesto(dto, idReserva, idCliente);

        await _repoPresupuestos.Add(presupuesto, ct); 

        return PresupuestoMapper.MapearAPresupuestoDto(presupuesto);
    }
}