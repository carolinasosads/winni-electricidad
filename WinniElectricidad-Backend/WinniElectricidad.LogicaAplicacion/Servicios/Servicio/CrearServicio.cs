using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Servicio;

public class CrearServicio : ICrearServicio
{
    private readonly IRepositorioServicio _repositorioServicio;

    public CrearServicio(IRepositorioServicio repositorioServicio)
    {
        _repositorioServicio = repositorioServicio;
    }
    
    public async Task<ServicioDto> Ejecutar(ServicioDto nuevoServicio, CancellationToken ct = default)
    {
        var servicio = ServicioMapper.MapearServicioDtoAEntidad(nuevoServicio);
        await _repositorioServicio.Add(servicio, ct);

        var dto = ServicioMapper.MapearServicioADto(servicio);
        
        return dto;
    }
}