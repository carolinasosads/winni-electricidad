namespace WinniElectricidad.Api.Servicio;

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
    /// <returns>URL relativa de la imagen almacenada (por ejemplo: <c>/resenias/imagen.jpg</c>).</returns>
    /// <exception cref="ArgumentException">
    /// Se lanza si el archivo es demasiado grande o no tiene un formato permitido.
    /// </exception>
    public async Task<string> GuardarAsync(IFormFile archivo)
    {
        if (archivo.Length > TamanioMaximo)
            throw new ArgumentException("La imagen supera el tamaño máximo permitido (5 MB).");

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

        if (!FormatosPermitidos.Contains(extension))
            throw new ArgumentException("Formato de imagen no permitido. Solo se aceptan JPG y PNG.");

        var carpeta = Path.Combine("wwwroot", "resenias");
        Directory.CreateDirectory(carpeta);

        var nombre = $"{Guid.NewGuid()}{extension}";
        var ruta = Path.Combine(carpeta, nombre);

        await using var stream = new FileStream(ruta, FileMode.Create);
        await archivo.CopyToAsync(stream);

        return $"/resenias/{nombre}";
    }
}