using WinniElectricidad.Compartido.DTOs.Reseñas;
using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;
using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.Compartido.DTOs.Mappers;

public class ReseñaMapper
{
    public static Reseña MapearNuevaReseñaDtoAEntidad(ReseñaACrearDto nuevaReseña, int idUsuario, string? imagenUrl)
    {
        return new Reseña(
            descripcion: nuevaReseña.Descripcion,
            calificacion: nuevaReseña.Calificacion,
            idUsuario,
            idServicio: nuevaReseña.IdServicio,
            imagenUrl
        );
    }
    
    public static ReseñaCreadaDto MapearAReseñaCreadaDto(Reseña reseña, UsuarioBase usuario, Servicio servicio)
    {
        return new ReseñaCreadaDto
        {
            IdReseña = reseña.Id,
            Descripcion = reseña.Descripcion,
            Calificacion = reseña.Calificacion,
            FechaReseña = reseña.FechaPublicacion,
            
            Cliente = new UsuarioReservaDto
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.NombreCompleto,
                Email = usuario.Email,
                Telefono = usuario.Telefono
            },

            Servicio = ServicioMapper.MapearServicioADto(servicio),
            
            ImagenUrl = reseña.ImagenUrl
        };
    }
}