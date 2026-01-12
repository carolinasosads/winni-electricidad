namespace WinniElectricidad.Api.Servicios;

/// <summary>
/// Define el contrato para el servicio encargado de almacenar imágenes en el servidor.
/// </summary>
/// <remarks>
/// El servicio que implemente esta interfaz debe validar el archivo recibido y devolver
/// la URL relativa del recurso almacenado.
/// </remarks>
public interface IServicioImagenes
{
    /// <summary>
    /// Guarda una imagen realizando validaciones de formato y tamaño.
    /// </summary>
    /// <param name="archivo">Archivo de imagen recibido como <see cref="IFormFile"/>.</param>
    /// <param name="carpeta">Carpeta donde se guardan las imágenes en Azure blob</param>
    /// <returns>URL de la imagen almacenada.</returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si el archivo supera el tamaño permitido o no tiene un formato válido.
    /// </exception>
    Task<string> GuardarImagenAsync(IFormFile archivo, string carpeta);
    /// <summary>
    /// Guarda una coleccion de imagenes realizando validaciones de formato y tamaño.
    /// </summary>
    /// <param name="archivos">Imagenes recibidas como <see cref="IFormFile"/>.</param>
    /// <param name="carpeta">Carpeta donde se guardan las imágenes en Azure blob</param>
    /// <param name="nombreServicio">Nombre con el que se guardarán las imagenes</param>
    /// <returns>URLs de las imagenes almacenadas.</returns>
    Task<ICollection<string>> GuardarImagenesAsync(ICollection<IFormFile> archivos, string nombreServicio, string carpeta);
    /// <summary>
    /// Elimina una coleccion de urls a imagenes.
    /// </summary>
    /// <param name="urls">Urls a eliminar.</param>
    Task EliminarImagenesAsync(IEnumerable<string> urls);
}