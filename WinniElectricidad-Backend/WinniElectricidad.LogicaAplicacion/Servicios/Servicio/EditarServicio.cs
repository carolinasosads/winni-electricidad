using WinniElectricidad.Compartido.DTOs.Mappers;
using WinniElectricidad.Compartido.DTOs.Servicios;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Servicio;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Servicio;

public class EditarServicio : IEditarServicio
{
    private readonly IRepositorioServicio  _repositorioServicio;

    public EditarServicio(IRepositorioServicio repositorioServicio)
    {
        _repositorioServicio = repositorioServicio;
    }
    public async Task<ServicioActivoDto> Ejecutar(int idServicio, EditarServicioDto servicio, ICollection<string>? imagenesUrl, CancellationToken ct = default)
    {
        var servicioAEditar = await _repositorioServicio.FindById(idServicio, ct);
        if (servicioAEditar == null)
        {
            throw new ArgumentException("El servicio no existe.");
        }
        
        if (servicio.UrlPrincipalExistente != null && servicioAEditar.Imagenes.All(i => i.Url != servicio.UrlPrincipalExistente))
            throw new ArgumentException("La imagen principal indicada no existe.");
        
        List<ServicioImagen>? nuevasImagenes = null;
        if (imagenesUrl is { Count: > 0 })
        {
            nuevasImagenes = ServicioMapper.MapearImagenesUrlAImagen(imagenesUrl);
        }
        
        string? urlPrincipalFinal = null;
        if (!string.IsNullOrWhiteSpace(servicio.UrlPrincipalExistente))
        {
            urlPrincipalFinal = servicio.UrlPrincipalExistente;
        } else if (servicio.IndexPrincipalNueva.HasValue && imagenesUrl != null)
        {
            var idx = servicio.IndexPrincipalNueva.Value;
            if (idx < 0 || idx >= imagenesUrl.Count)
                throw new ArgumentException("Índice de imagen principal nueva inválido.");

            urlPrincipalFinal = imagenesUrl.ElementAt(idx);
        }
        
        servicioAEditar.Actualizar(servicio.Titulo, servicio.Descripcion, servicio.ImagenesExistentes, nuevasImagenes, urlPrincipalFinal);
        await _repositorioServicio.Update(servicioAEditar, ct);

        var servicioEditado = ServicioMapper.MapearServicioAActivoDto(servicioAEditar);
        return servicioEditado;
    }
}