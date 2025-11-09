
namespace WinniElectricidad.Compartido.DTOs.Seguridad
{
    
    // Respuesta de https://api.hcaptcha.com/siteverify
    public sealed record HCaptchaVerifyResponse
    {
        public bool Success { get; init; }
        public DateTime? Challenge_TS { get; init; }
        public string? Hostname { get; init; }
        public string[]? Error_Codes { get; init; }
    }
}