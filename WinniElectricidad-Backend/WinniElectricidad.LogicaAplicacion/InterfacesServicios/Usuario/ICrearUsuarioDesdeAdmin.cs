using WinniElectricidad.Compartido.DTOs.Registro;
using WinniElectricidad.Compartido.DTOs.Usuarios;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Usuario;

public interface ICrearUsuarioDesdeAdmin
{
    Task<UsuarioCreadoDesdeAdminDto?> CrearUsuario(UsuarioRegistroAdminDto dto, CancellationToken ct = default);}