using WinniElectricidad.Compartido.DTOs.Usuarios.Busqueda;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Usuario;

public class BuscarUsuarios : IBuscarUsuarios
{
    private readonly IRepositorioUsuario _repositorioUsuario;

    public BuscarUsuarios(IRepositorioUsuario repositorioUsuario)
    {
        _repositorioUsuario = repositorioUsuario;
    }

    public async Task<ICollection<UsuarioBusquedaDto>> BuscarUsuariosAsync(string query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<UsuarioBusquedaDto>();
        }
        
        var usuarios = await _repositorioUsuario.BuscarPorNombreEmailTelefono(query, ct);

        return usuarios
            .Select(u => new UsuarioBusquedaDto
            {
                Id = u.IdUsuario,
                NombreCompleto = u.NombreCompleto,
                Email = u.Email,
                Telefono = u.Telefono
            })
            .ToList();
    }
}