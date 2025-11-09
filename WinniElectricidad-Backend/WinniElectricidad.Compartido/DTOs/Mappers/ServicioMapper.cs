using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.DTOs.Mappers;

public class ServicioMapper
{
    public static ICollection<ServicioDisponibleDto> MapearServiciosADtos(
        IEnumerable<Servicio> serviciosDisponibles)
    {
        return serviciosDisponibles.Select(servicio => new ServicioDisponibleDto() { Id = servicio.Id, Titulo = servicio.Titulo, }).ToList();
    }
}