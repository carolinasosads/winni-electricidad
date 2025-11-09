using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Servicio;

public class ObtenerServiciosActivos : IObtenerServiciosActivos
{
    private readonly IRepositorioServicio _repositorioServicio;

    public ObtenerServiciosActivos(IRepositorioServicio repositorioServicio)
    {
        _repositorioServicio = repositorioServicio;
    }
    public async Task<IEnumerable<ServicioActivoDto>> Ejecutar(CancellationToken ct = default)
    {
        var serviciosDisponibles = await _repositorioServicio.FindAllActive(ct);
        
        if (serviciosDisponibles == null) return new List<ServicioActivoDto>();
        var serviciosDisponiblesDto = ServicioMapper.MapearServiciosADtos(serviciosDisponibles);
        
        return serviciosDisponiblesDto;
    }
}