using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.DTOs.Mappers;

public static class ServicioMapper
{
    public static ICollection<ServicioActivoDto> MapearServiciosADtos(IEnumerable<Servicio> serviciosActivos)
    {
        return serviciosActivos
            .Select(servicio => new ServicioActivoDto
            {
                Id = servicio.Id,
                Titulo = servicio.Titulo,
                Descripcion = servicio.Descripcion,
                ImagenUrl = servicio.ImagenUrl
            })
            .ToList();
    }

    public static ServicioActivoDto MapearServicioADto(Servicio servicio)
    {
        return new ServicioActivoDto
        {
            Id = servicio.Id,
            Titulo = servicio.Titulo,
            Descripcion = servicio.Descripcion,
            ImagenUrl = servicio.ImagenUrl
        };
    }
}