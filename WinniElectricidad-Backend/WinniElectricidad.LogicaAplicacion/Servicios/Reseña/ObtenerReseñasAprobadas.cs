using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Reseñas;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reseñas;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reseña;

public class ObtenerReseñasAprobadas : IObtenerReseñasAprobadas
{
    private readonly IRepositorioReseña _repositorioReseña;
    private readonly IRepositorioServicio _repositorioServicio;
    private readonly IRepositorioUsuario _repositorioUsuario;

    public ObtenerReseñasAprobadas(IRepositorioReseña repositorioReseña,  IRepositorioServicio repositorioServicio,  IRepositorioUsuario repositorioUsuario)
    {
        _repositorioReseña = repositorioReseña;
        _repositorioServicio = repositorioServicio;
        _repositorioUsuario =  repositorioUsuario;
    }
    
    public async Task<IEnumerable<ReseñaCreadaDto>> Ejecutar(CancellationToken ct = default)
    {
        var reseñasAprobadas = await _repositorioReseña.FindAll(ct);
        
        var idsUsuarios = reseñasAprobadas
            .Select(r => r.IdUsuario)
            .Distinct()
            .ToList();

        var idsServicios = reseñasAprobadas
            .Select(r => r.IdServicio)
            .Distinct()
            .ToList();

        var usuarios = await _repositorioUsuario.FindByIds(idsUsuarios, ct);
        var servicios = await _repositorioServicio.FindByIds(idsServicios, ct);

        var usuariosPorId = usuarios.ToDictionary(u => u.IdUsuario);
        var serviciosPorId = servicios.ToDictionary(s => s.Id);

        var reseñasAprobadasDto = new List<ReseñaCreadaDto>(reseñasAprobadas.Count);

        foreach (var reseña in reseñasAprobadas)
        {
            if (!usuariosPorId.TryGetValue(reseña.IdUsuario, out var usuario))
            {
                throw new ReseñaException(
                    $"Usuario {reseña.IdUsuario} no encontrado para la reseña {reseña.Id}."
                );
            }

            if (!serviciosPorId.TryGetValue(reseña.IdServicio, out var servicio))
            {
                throw new ReseñaException(
                    $"Servicio {reseña.IdServicio} no encontrado para la reseña {reseña.Id}."
                );
            }

            reseñasAprobadasDto.Add(
                ReseñaMapper.MapearAReseñaCreadaDto(reseña, usuario, servicio)
            );
        }

        return reseñasAprobadasDto;
    }
}