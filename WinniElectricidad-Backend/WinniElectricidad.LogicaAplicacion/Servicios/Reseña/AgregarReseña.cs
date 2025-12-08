using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Reseñas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reseñas;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reseña;

public class AgregarReseña : IAgregarReseña
{
    private readonly IRepositorioReseña _repositorioReseña;
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IRepositorioServicio _repositorioServicio;

    public AgregarReseña(IRepositorioReseña repositorioReseña, IRepositorioUsuario repositorioUsuario,  IRepositorioServicio repositorioServicio)
    {
        _repositorioReseña = repositorioReseña;
        _repositorioUsuario = repositorioUsuario;
        _repositorioServicio = repositorioServicio;
    }

    public async Task<ReseñaCreadaDto> Ejecutar(ReseñaACrearDto nuevaReseña, int idUsuario, string? imagenUrl,
        CancellationToken ct)
    {
        var usuario = await _repositorioUsuario.FindById(idUsuario, ct);
        if (usuario == null)
            throw new UnauthorizedAccessException("El usuario no existe o el token es inválido.");

        var servicio = await _repositorioServicio.FindById(nuevaReseña.IdServicio, ct);

        if (servicio is null)
        {
            throw new ReseñaException("Debes seleccionar un servicio válido.");
        }
        
        var reseña = ReseñaMapper.MapearNuevaReseñaDtoAEntidad(nuevaReseña, idUsuario, imagenUrl);
        await _repositorioReseña.Add(reseña, ct);
        
        var dto = ReseñaMapper.MapearAReseñaCreadaDto(reseña, usuario, servicio);
        
        return dto;
    }
}