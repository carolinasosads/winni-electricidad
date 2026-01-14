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
                Activo = servicio.Activo,
                Icono = servicio.Icono
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
            Icono = servicio.Icono,
        };
    }
    
    public static ServicioDto MapearServicioADto(Servicio servicio)
    {
        return new ServicioDto
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
            Icono = servicio.Icono,
            Activo = servicio.Activo
        };
    }
    
    public static Servicio MapearServicioDtoAEntidad(CrearServicioDto servicio, ICollection<string> imagenesUrl)
    {
        var imagenes = imagenesUrl
            .Select((url, index) =>
                new ServicioImagen(
                    url,
                    esPrincipal: index == 0
                )
            )
            .ToList();
        
        return new Servicio(servicio.Titulo, servicio.Descripcion, imagenes, servicio.Icono);
    }
    
    public static List<ServicioImagen> MapearImagenesUrlAImagen(ICollection<string> urls)
    {
        return urls
            .Select(url => new ServicioImagen(url, esPrincipal: false))
            .ToList();
    }
}