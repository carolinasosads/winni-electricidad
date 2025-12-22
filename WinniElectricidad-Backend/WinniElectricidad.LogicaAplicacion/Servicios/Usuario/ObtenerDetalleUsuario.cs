using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Usuarios.ListadoUsuarios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class ObtenerDetalleUsuario: IObtenerDetalleUsuario
{
    private readonly IRepositorioUsuario _repositorioUsuario;

    public ObtenerDetalleUsuario(IRepositorioUsuario repo)
    {
        _repositorioUsuario = repo;
    }

    public async Task<DetalleUsuariosDto?> Execute(int idUsuario, CancellationToken ct = default)
    {
        var cliente = await _repositorioUsuario.FindClienteDetalleById(idUsuario, ct);
        if (cliente is null) return null;

        return UsuarioMapper.MapearADetalleUsuariosDto(cliente);
    }
}