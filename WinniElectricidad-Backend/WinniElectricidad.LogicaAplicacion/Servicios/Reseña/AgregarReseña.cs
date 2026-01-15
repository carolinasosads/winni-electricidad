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
    
    private readonly IModeracionOpenAi _moderacionOpenAi;
    private readonly IEvaluarPuntajeResenia _evaluarPuntajeResenia;
    
    public AgregarReseña(IRepositorioReseña repositorioReseña, IRepositorioUsuario repositorioUsuario,  IRepositorioServicio repositorioServicio, IModeracionOpenAi moderacionOpenAi, IEvaluarPuntajeResenia evaluarPuntajeResenia)
    {
        _repositorioReseña = repositorioReseña;
        _repositorioUsuario = repositorioUsuario;
        _repositorioServicio = repositorioServicio;
        _moderacionOpenAi = moderacionOpenAi;
        _evaluarPuntajeResenia = evaluarPuntajeResenia;
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

        var esOfensiva = await _moderacionOpenAi.EsOfensiva(nuevaReseña.Descripcion);

        if (esOfensiva)
        {
            throw new ReseñaOfensivaException("La reseña no cumple con las normas establecidas.");
        }
        
        var reseña = ReseñaMapper.MapearNuevaReseñaDtoAEntidad(nuevaReseña, idUsuario, imagenUrl);
       
        var puntaje = await _evaluarPuntajeResenia.CalcularPuntajeIa(nuevaReseña.Descripcion, nuevaReseña.Calificacion, ct);

        reseña.PuntajeReseniaIa = puntaje;

        await _repositorioReseña.Add(reseña, ct);
        
        var dto = ReseñaMapper.MapearAReseñaCreadaDto(reseña, usuario, servicio);
        
        return dto;
    }
}