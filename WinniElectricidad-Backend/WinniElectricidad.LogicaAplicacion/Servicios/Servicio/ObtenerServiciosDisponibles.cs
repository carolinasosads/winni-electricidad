using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Servicio;

public class ObtenerServiciosDisponibles : IObtenerServiciosDisponibles
{
    private readonly IRepositorioServicio _repositorioServicio;

    public ObtenerServiciosDisponibles(IRepositorioServicio repositorioServicio)
    {
        _repositorioServicio = repositorioServicio;
    }
    public async Task<IEnumerable<ServicioDisponibleDto>> Ejecutar(CancellationToken ct = default)
    {
        var serviciosDisponibles = await _repositorioServicio.FindAllActive(ct);
        
        if (serviciosDisponibles == null) return new List<ServicioDisponibleDto>();
        var serviciosDisponiblesDto = ServicioMapper.MapearServiciosADtos(serviciosDisponibles);
        
        return serviciosDisponiblesDto;
    }
}