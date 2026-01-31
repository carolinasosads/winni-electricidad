namespace WinniElectricidad.Api.Servicios;

/// <summary>
/// 
/// </summary>
public class CorsOptions
{
    public string[] AllowedOrigins { get; set; } = [];
    public string[] AllowedPreviewHostSuffixes { get; set; } = [];
}