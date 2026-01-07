using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace WinniElectricidad.Api.Servicios;

/// <summary>
/// Servicio encargado de gestionar el almacenamiento de imágenes en el servidor.
/// </summary>
/// <remarks>
/// Este servicio forma parte de la capa de infraestructura de la API, ya que su función es
/// interactuar con el sistema de archivos para guardar imágenes asociadas a reseñas u otras entidades.
///
/// No contiene lógica de negocio:  
/// - Valida formato y tamaño de los archivos recibidos.  
/// - Genera un nombre único para cada imagen.  
/// - Almacena los archivos en <c>wwwroot/resenias</c>.  
/// - Devuelve una URL relativa para su posterior uso desde el frontend.
///
/// Implementa <see cref="IServicioImagenes"/> como contrato para su uso en controladores y servicios.
/// </remarks>
public class ServicioImagenes : IServicioImagenes
{
    private static readonly string[] FormatosPermitidos = { ".jpg", ".jpeg", ".png" };
    private const long TamanioMaximo = 5 * 1024 * 1024;
    
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "imagenes";
    
    /// <summary>
    /// Constructor que recibe el cliente de Azure Blob Storage por inyección de dependencias.
    /// </summary>
    /// <param name="blobServiceClient">Cliente de Azure Blob Storage inyectado mediante dependencias.</param>
    public ServicioImagenes(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    /// <summary>
    /// Guarda una imagen en el servidor realizando validaciones de formato y tamaño.
    /// </summary>
    /// <remarks>
    /// Valida que el archivo tenga un tamaño máximo de 5 MB y que su extensión sea JPG o PNG.
    /// Si la validación es correcta, el archivo se almacena en <c>wwwroot/resenias</c> con un nombre único
    /// y se devuelve la URL relativa para su posterior uso por el frontend.
    /// 
    /// **Formatos permitidos:** .jpg, .jpeg, .png  
    /// **Tamaño máximo:** 5 MB
    /// 
    /// **Excepciones:**
    /// - <see cref="ArgumentException"/>: cuando el archivo supera el tamaño permitido o su formato no es válido.
    /// </remarks>
    /// <param name="archivo">Archivo de imagen recibido como <see cref="IFormFile"/>.</param>
    /// <param name="carpeta">Carpeta donde se guardan las imágenes en Azure blob</param>
    /// <returns>URL relativa de la imagen almacenada (por ejemplo: <c>/resenias/imagen.jpg</c>).</returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si el archivo es demasiado grande o no tiene un formato permitido.
    /// </exception>
    public async Task<string> GuardarAsync(IFormFile archivo, string carpeta)
    {
        if (archivo == null || archivo.Length == 0)
            throw new ArgumentException("Para ser procesado, el archivo de imagen es obligatorio y no puede estar vacío.");
        
        if (archivo.Length > TamanioMaximo)
            throw new ArgumentException("La imagen supera el tamaño máximo permitido (5 MB).");

        var sanitizedFileName = Path.GetFileName(archivo.FileName);
        var extension = Path.GetExtension(sanitizedFileName).ToLowerInvariant();

        if (!FormatosPermitidos.Contains(extension))
            throw new ArgumentException("Formato de imagen no permitido. Solo se aceptan JPG y PNG.");

        var nombre = $"{Guid.NewGuid()}{extension}";
        var blobPath = $"{carpeta}/{nombre}";

        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);
        var blob = container.GetBlobClient(blobPath);

        await using var stream = archivo.OpenReadStream();
        await blob.UploadAsync(stream, new BlobHttpHeaders
        {
            ContentType = archivo.ContentType
        });

        return blob.Uri.ToString();
    }
}