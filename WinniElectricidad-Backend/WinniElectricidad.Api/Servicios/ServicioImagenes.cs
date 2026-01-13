using System.Text.RegularExpressions;
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

    private readonly ILogger<ServicioImagenes> _logger;
    
    /// <summary>
    /// Constructor que recibe el cliente de Azure Blob Storage por inyección de dependencias.
    /// </summary>
    /// <param name="blobServiceClient">Cliente de Azure Blob Storage inyectado mediante dependencias.</param>
    /// <param name="logger">Logger de errores.</param>

    public ServicioImagenes(BlobServiceClient blobServiceClient, ILogger<ServicioImagenes> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
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
    public async Task<string> GuardarImagenAsync(IFormFile archivo, string carpeta)
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
    
    /// <summary>
    /// Guarda múltiples imágenes asociadas a un servicio, generando
    /// nombres secuenciales para evitar colisiones.
    /// </summary>
    /// <param name="archivos">Colección de archivos de imagen.</param>
    /// <param name="nombreServicio">Nombre del servicio asociado.</param>
    /// <param name="carpeta">Carpeta lógica dentro del contenedor.</param>
    /// <returns>
    /// Lista de URLs absolutas de las imágenes almacenadas.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si no se envían imágenes o si alguna no cumple
    /// con las validaciones de tamaño o formato.
    /// </exception>
    public async Task<ICollection<string>> GuardarImagenesAsync(
        ICollection<IFormFile> archivos,
        string nombreServicio,
        string carpeta)
    {
        if (archivos == null || archivos.Count == 0)
            throw new ArgumentException("Debe enviarse al menos una imagen.");
        
        var archivosValidos = archivos
            .Where(a => a.Length > 0)
            .ToList();

        if (archivosValidos.Count == 0)
            throw new ArgumentException("Debe enviarse al menos una imagen válida.");

        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);
        var urls = new List<string>();

        var nombreBase = NormalizarNombre(nombreServicio);
        
        var ultimoIndice = await ObtenerUltimoIndiceAsync(container, carpeta, nombreBase);
        var indice = ultimoIndice + 1;

        foreach (var archivo in archivosValidos)
        {
            switch (archivo.Length)
            {
                case 0:
                    continue;
                case > TamanioMaximo:
                    throw new ArgumentException("Una de las imágenes supera el tamaño máximo permitido (5 MB).");
            }

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!FormatosPermitidos.Contains(extension))
                throw new ArgumentException("Formato de imagen no permitido. Solo JPG y PNG.");

            var nombre =
                indice == 1
                    ? $"{nombreBase}{extension}"
                    : $"{nombreBase}{indice}{extension}";

            var blobPath = $"{carpeta}/{nombre}";
            var blob = container.GetBlobClient(blobPath);

            await using var stream = archivo.OpenReadStream();
            await blob.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = archivo.ContentType
            });

            urls.Add(blob.Uri.ToString());
            indice++;
        }

        return urls;
    }
    
    /// <summary>
    /// Elimina un conjunto de imágenes a partir de sus URLs.
    /// </summary>
    /// <remarks>
    /// El proceso es tolerante a fallos parciales:
    /// si una imagen no puede eliminarse, el error se registra
    /// pero la operación continúa para el resto.
    /// Esto evita que errores de infraestructura afecten
    /// la operación principal del sistema.
    /// </remarks>
    /// <param name="urls">Colección de URLs de imágenes a eliminar.</param>
    public async Task EliminarImagenesAsync(IEnumerable<string> urls)
    {
        if (urls == null)
            return;
        
        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);

        foreach (var url in urls)
        {
            try
            {
                var uri = new Uri(url);
                var blobName = uri.AbsolutePath
                    .Replace($"/{ContainerName}/", "", StringComparison.OrdinalIgnoreCase)
                    .TrimStart('/');

                var blobClient = container.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando imagen {Url}", url);
            }
        }
    }

    /// <summary>
    /// Obtiene el índice más alto utilizado en los nombres de imágenes
    /// de un servicio para evitar sobrescrituras.
    /// </summary>
    /// <param name="container">Contenedor de blobs.</param>
    /// <param name="carpeta">Carpeta lógica.</param>
    /// <param name="nombreBase">Nombre base normalizado.</param>
    /// <returns>
    /// Último índice encontrado o cero si no existen imágenes previas.
    /// </returns>
    private async Task<int> ObtenerUltimoIndiceAsync(
        BlobContainerClient container,
        string carpeta,
        string nombreBase)
    {
        var prefix = $"{carpeta}/{nombreBase}";
        var maxIndice = 0;

        await foreach (var blob in container.GetBlobsAsync(prefix: prefix))
        {
            var nombre = Path.GetFileNameWithoutExtension(blob.Name);

            if (nombre.Equals(nombreBase, StringComparison.OrdinalIgnoreCase))
            {
                maxIndice = Math.Max(maxIndice, 1);
                continue;
            }

            var match = Regex.Match(nombre, $"{nombreBase}(\\d+)$");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var idx))
            {
                maxIndice = Math.Max(maxIndice, idx);
            }
        }

        return maxIndice;
    }
    
    private static string NormalizarNombre(string nombre)
    {
        nombre = nombre.Trim().ToLowerInvariant();

        nombre = nombre
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ú", "u")
            .Replace("ñ", "n");

        nombre = Regex.Replace(nombre, @"[^a-z0-9]+", "-");

        return nombre.Trim('-');
    }
}