using WinniElectricidad.Compartido.DTOs.Reseñas;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;

public interface IAgregarReseña
{
    Task<ReseñaCreadaDto> Ejecutar(ReseñaACrearDto nuevaReseña, int idUsuario, string? imagenUrl, CancellationToken ct);
}