using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Servicio;

public class ObtenerServiciosSegunEstado : IObtenerServiciosSegunEstado
{
    private readonly IRepositorioServicio _repositorioServicio;

    public ObtenerServiciosSegunEstado(IRepositorioServicio repositorioServicio)
    {
        _repositorioServicio = repositorioServicio;
    }
    public async Task<IEnumerable<ServicioActivoDto>> Ejecutar(bool activo, CancellationToken ct = default)
    {
        var serviciosDisponibles = await _repositorioServicio.FindAllSegunEstado(activo, ct);
        
        var serviciosDisponiblesDto = ServicioMapper.MapearServiciosADtos(activo, serviciosDisponibles);
        
        return serviciosDisponiblesDto;
    }
}