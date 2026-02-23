using Microsoft.AspNetCore.Http;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Imagenes;

/// <summary>
/// Define el contrato para el servicio encargado de almacenar imágenes en el proveedor de almacenamiento en la nube.
/// </summary>
/// <remarks>
/// Esta interfaz se ubica en la capa de <b>lógica de aplicación</b> para que pueda ser referenciada
/// desde casos de uso o controladores sin acoplarlos a la implementación concreta.
///
/// La implementación concreta (<c>ServicioImagenes</c>) reside en la capa de infraestructura (<b>API</b>)
/// porque depende de <see cref="IFormFile"/> (tipo de ASP.NET Core) y del SDK de <b>Azure Blob Storage</b>,
/// ambos detalles de infraestructura que no deben filtrarse a la lógica de negocio.
///
/// Este patrón (interfaz en aplicación, implementación en infraestructura) es el mismo que se aplica a
/// <c>IHCaptchaVerifier</c> / <c>HCaptchaServicio</c>: la abstracción pertenece a la capa interior
/// y la dependencia concreta del servicio externo permanece en la capa exterior.
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
    /// Guarda una colección de imágenes realizando validaciones de formato y tamaño.
    /// </summary>
    /// <param name="archivos">Imagenes recibidas como <see cref="IFormFile"/>.</param>
    /// <param name="carpeta">Carpeta donde se guardan las imágenes en Azure blob</param>
    /// <param name="nombreServicio">Nombre con el que se guardarán las imagenes</param>
    /// <returns>URLs de las imágenes almacenadas.</returns>
    Task<ICollection<string>> GuardarImagenesAsync(ICollection<IFormFile> archivos, string nombreServicio, string carpeta);

    /// <summary>
    /// Elimina una colección de URLs de imágenes.
    /// </summary>
    /// <param name="urls">URLs a eliminar.</param>
    Task EliminarImagenesAsync(IEnumerable<string> urls);
}
