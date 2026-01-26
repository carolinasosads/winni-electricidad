using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Usuarios.ListadoUsuarios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class ObtenerUsuariosPorServicio : IObtenerUsuariosPorServicio
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IRepositorioServicio _repositorioServicio;

    public ObtenerUsuariosPorServicio(IRepositorioUsuario repositorioUsuario,  IRepositorioServicio repositorioServicio)
    {
        _repositorioUsuario = repositorioUsuario;
        _repositorioServicio = repositorioServicio;
    }
    
    public async Task<IEnumerable<ListadoUsuariosDto>> Ejecutar(int idServicio, CancellationToken ct)
    {
        var servicio = await _repositorioServicio.FindById(idServicio, ct);

        if (servicio == null)
        {
            throw new ArgumentException("No se encontró el servicio");
        }
        
        var clientes = await _repositorioUsuario.FindAllFilteredByService(idServicio, ct);
        
        return UsuarioMapper.MapearAListadoUsuariosDto(clientes);
    }
}