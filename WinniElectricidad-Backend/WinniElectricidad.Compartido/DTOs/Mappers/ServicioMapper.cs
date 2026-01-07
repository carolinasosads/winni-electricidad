using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.DTOs.Mappers;

public static class ServicioMapper
{
    public static IEnumerable<ServicioActivoDto> MapearServiciosADtos(bool activo, IEnumerable<Servicio> servicios)
    {
        if (activo)
        {
            return servicios
                .Select(servicio => new ServicioActivoDto
                {
                    Id = servicio.Id,
                    Titulo = servicio.Titulo,
                    Descripcion = servicio.Descripcion,
                    Imagenes = servicio.Imagenes
                        .Select(i => new ServicioImagenDto {
                            Url = i.Url,
                            EsPrincipal = i.EsPrincipal,
                        })
                        .ToList()
                })
                .ToList();
        }
        
        return servicios
            .Select(servicio => new ServicioDto()
            {
                Id = servicio.Id,
                Titulo = servicio.Titulo,
                Descripcion = servicio.Descripcion,
                Imagenes = servicio.Imagenes
                    .Select(i => new ServicioImagenDto {
                        Url = i.Url,
                        EsPrincipal = i.EsPrincipal,
                    })
                    .ToList(),
                Activo = servicio.Activo
            })
            .ToList();
    }

    public static ServicioActivoDto MapearServicioAActivoDto(Servicio servicio)
    {
        return new ServicioActivoDto
        {
            Id = servicio.Id,
            Titulo = servicio.Titulo,
            Descripcion = servicio.Descripcion,
            Imagenes = servicio.Imagenes
                .Select(i => new ServicioImagenDto {
                    Url = i.Url,
                    EsPrincipal = i.EsPrincipal,
                })
                .ToList(),
        };
    }
    
    public static ServicioDto MapearServicioADto(Servicio servicio)
    {
        return new ServicioDto
        {
            Id = servicio.Id,
            Titulo = servicio.Titulo,
            Descripcion = servicio.Descripcion,
            ImagenUrl = servicio.ImagenUrl,
            Activo = servicio.Activo
        };
    }
    
    public static Servicio MapearServicioDtoAEntidad(ServicioDto servicio)
    {
        return new Servicio
        {
            Titulo = servicio.Titulo,
            Descripcion = servicio.Descripcion,
            ImagenUrl = servicio.ImagenUrl
        };
    }
}