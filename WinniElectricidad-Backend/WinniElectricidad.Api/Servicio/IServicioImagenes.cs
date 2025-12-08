namespace WinniElectricidad.Api.Servicio;

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
    /// <returns>URL relativa de la imagen almacenada.</returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si el archivo supera el tamaño permitido o no tiene un formato válido.
    /// </exception>
    Task<string> GuardarAsync(IFormFile archivo);
}