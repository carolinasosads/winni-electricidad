using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Usuarios.ListadoUsuarios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class ListarTodosLosUsuarios: IListarTodosLosUsuarios
{
    private readonly IRepositorioUsuario _repositorioUsuario;

    public ListarTodosLosUsuarios(IRepositorioUsuario repoUsuario)
    {
        _repositorioUsuario = repoUsuario;
    }

    public async Task<IReadOnlyList<ListadoUsuariosDto>> Listar(CancellationToken ct = default)
    {
        var clientes = await _repositorioUsuario.FindAll(ct);
        
        return UsuarioMapper.MapearAListadoUsuariosDto(clientes);
    }
}
