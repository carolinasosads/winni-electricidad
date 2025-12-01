using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.DTOs.Mappers;

public static class ServicioMapper
{
    public static ICollection<ServicioActivoDto> MapearServiciosADtos(
        IEnumerable<Servicio> serviciosActivos)
    {
        return serviciosActivos.Select(servicio => new ServicioActivoDto() { Id = servicio.Id, Titulo = servicio.Titulo }).ToList();
    }
}