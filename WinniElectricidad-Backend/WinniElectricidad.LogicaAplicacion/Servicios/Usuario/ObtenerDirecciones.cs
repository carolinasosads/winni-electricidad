using WinniElectricidad.Compartido.DTOs.Direcciones;
using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class ObtenerDirecciones : IObtenerDirecciones
{
    private readonly IRepositorioUsuario _repositorioUsuario;

    public ObtenerDirecciones(IRepositorioUsuario repositorioUsuario)
    {
        _repositorioUsuario = repositorioUsuario;
    }
    
    public async Task<IEnumerable<DireccionDetalleDto>> Ejecutar(int idUsuario, CancellationToken ct = default)
    {
        var direcciones = await _repositorioUsuario.FindAddressByUserId(idUsuario, ct);

        var direccionesDto = DireccionMapper.MapearDireccionesADtos(direcciones);

        return direccionesDto;
    }
}