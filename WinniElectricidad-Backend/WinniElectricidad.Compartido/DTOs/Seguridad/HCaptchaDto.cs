
namespace WinniElectricidad.Compartido.DTOs.Seguridad
{
    
    // Respuesta de https://api.hcaptcha.com/siteverify
    public sealed class HCaptchaVerifyResponse
    {
        public bool Success { get; set; }
        public DateTime? Challenge_TS { get; set; }
        public string? Hostname { get; set; }
        public string[]? Error_Codes { get; set; }
    }
}