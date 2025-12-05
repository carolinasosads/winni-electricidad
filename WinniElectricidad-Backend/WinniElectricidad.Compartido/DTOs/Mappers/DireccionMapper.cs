using WinniElectricidad.Compartido.DTOs.Direcciones;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.DTOs.Mappers;

public static class DireccionMapper
{
    public static ICollection<DireccionDetalleDto> MapearDireccionesADtos(
        IEnumerable<Direccion> direcciones)
    {
        return direcciones.Select(direccion => new DireccionDetalleDto()
        {
            Id = direccion.IdDireccion,
            Calle = direccion.Calle,
            Esquina = direccion.Esquina,
            Apto = direccion.Apto,
            Numero = direccion.Numero
        }).ToList();
    }
    public static DireccionDto MapearDireccionADto(Direccion direccion)
    {
        return new DireccionDto
        {
            Calle = direccion.Calle,
            Esquina = direccion.Esquina,
            Apto = direccion.Apto,
            Numero = direccion.Numero
        };
    }
}