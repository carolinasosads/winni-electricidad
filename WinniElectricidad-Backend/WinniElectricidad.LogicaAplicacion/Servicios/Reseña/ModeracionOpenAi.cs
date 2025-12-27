using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using WinniElectricidad.Compartido.Configuracion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;
using WinniElectricidad.LogicaNegocio.ExcepcionesPersonalizadas.Reseñas;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reseña;

public class ModeracionOpenAi : IModeracionOpenAi
{
    private readonly OpenAIClient _client;
    private readonly OpenAiOptions _options;
    private readonly ILogger<ModeracionOpenAi> _logger;

    public ModeracionOpenAi(OpenAIClient client, ILogger<ModeracionOpenAi> logger, IOptions<OpenAiOptions> options)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }
    
    private sealed class ModeracionIaResultado
    {
        public bool Rechazar { get; set; }
        public string Motivo { get; set; } = "ninguno";
    }

    public async Task<bool> EsOfensiva(string texto)
    {
        var chatClient = _client.GetChatClient(_options.Model);

        var prompt =
            "Analizá el siguiente texto.\n\n" +
            "Respondé EXCLUSIVAMENTE en formato JSON (sin texto adicional, sin markdown), con esta estructura exacta:\n" +
            "{\n" +
            "  \"rechazar\": true,\n" +
            "  \"motivo\": \"insulto|discriminacion|politica|religion|futbol|sexual|violencia|minorias|ninguno\"\n" +
            "}\n\n" +
            "Reglas (en cualquier idioma):\n" +
            "- \"rechazar\": true si el texto contiene al menos uno de estos elementos:\n" +
            "  1) Insultos, agravios o lenguaje ofensivo.\n" +
            "  2) Mensajes discriminatorios o de odio hacia personas o grupos.\n" +
            "  3) Referencias o comentarios sobre política, partidos políticos, ideologías o líderes políticos.\n" +
            "  4) Referencias o comentarios sobre religión, creencias religiosas o figuras religiosas.\n" +
            "  5) Comentarios sobre fútbol u otros deportes.\n" +
            "  6) Contenido sexual o insinuaciones sexuales.\n" +
            "  7) Mensajes que promuevan violencia, guerra, amenazas o daño físico.\n" +
            "  8) Ataques o referencias negativas hacia minorías de cualquier tipo.\n\n" +
            "- \"rechazar\": false únicamente si el texto es neutral, respetuoso y relevante como reseña de un servicio.\n" +
            "- Si \"rechazar\" es false, el \"motivo\" DEBE ser \"ninguno\".\n\n" +
            "Texto a analizar:\n" +
            "\"\"\"\n" +
            texto + "\n" +
            "\"\"\"";
        
        var motivosValidos = new[]
        {
            "insulto", "discriminacion", "politica", "religion",
            "futbol", "sexual", "violencia", "minorias", "ninguno"
        };
        
        try
        {
            var response = await chatClient.CompleteChatAsync(new UserChatMessage(prompt));
            
            if (response.Value.Content.Count == 0)
            {
                _logger.LogWarning("Respuesta vacía de IA");
                throw new ModeracionIaNoDisponibleException("Respuesta vacía de la IA.");
            }
            
            var raw = response.Value.Content[0].Text.Trim();

            raw = raw.Replace("```json", "").Replace("```", "").Trim();

            var result = JsonSerializer.Deserialize<ModeracionIaResultado>(
                raw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (result is null || string.IsNullOrWhiteSpace(result.Motivo))
            {
                _logger.LogWarning("Respuesta inválida de IA en moderación: {Raw}", raw);
                throw new ModeracionIaNoDisponibleException("Respuesta inválida de la IA de moderación.");
            }
            
            if (!motivosValidos.Contains(result.Motivo))
            {
                _logger.LogWarning("Motivo inesperado devuelto por IA: {Motivo}", result.Motivo);
                throw new ModeracionIaNoDisponibleException("Motivo inválido devuelto por IA.");
            }

            result.Motivo = result.Motivo.Trim().ToLowerInvariant();

            if (!result.Rechazar && result.Motivo != "ninguno")
            {
                _logger.LogWarning("IA devolvió motivo inesperado cuando rechazar=false. Raw: {Raw}", raw);
                result.Motivo = "ninguno";
            }

            _logger.LogInformation(
                "Reseña evaluada por IA. Rechazar={Rechazar}, Motivo={Motivo}",
                result.Rechazar,
                result.Motivo
            );

            return result.Rechazar;
        }
        catch (ModeracionIaNoDisponibleException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al evaluar reseña con IA");
            throw new ModeracionIaNoDisponibleException("No se pudo validar la reseña con IA.", ex);
        }
    }
}