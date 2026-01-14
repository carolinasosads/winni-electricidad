using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Servicios;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Servicio;

public class CrearServicio : ICrearServicio
{
    private readonly IRepositorioServicio _repositorioServicio;

    public CrearServicio(IRepositorioServicio repositorioServicio)
    {
        _repositorioServicio = repositorioServicio;
    }
    
    public async Task<ServicioDto> Ejecutar(CrearServicioDto nuevoServicio, ICollection<string> imagenesUrl, CancellationToken ct = default)
    {
        var existeServicio = await _repositorioServicio.FindByTitulo(nuevoServicio.Titulo, ct);

        if (existeServicio is not null)
        {
            throw new ServicioDuplicadoException("Ya existe un servicio de " + nuevoServicio.Titulo);
        }
        
        var servicio = ServicioMapper.MapearServicioDtoAEntidad(nuevoServicio, imagenesUrl);
        await _repositorioServicio.Add(servicio, ct);

        var dto = ServicioMapper.MapearServicioADto(servicio);
        
        return dto;
    }
}