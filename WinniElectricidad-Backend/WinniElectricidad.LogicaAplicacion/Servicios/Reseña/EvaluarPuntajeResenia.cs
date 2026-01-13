using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using WinniElectricidad.Compartido.Configuracion;
using WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reseña;

namespace WinniElectricidad.LogicaAplicacion.Servicios.Reseña;

public class EvaluarPuntajeResenia : IEvaluarPuntajeResenia
{
    private readonly OpenAIClient _client;
    private readonly OpenAiOptions _options;
    private readonly ILogger<EvaluarPuntajeResenia> _logger;

    public EvaluarPuntajeResenia(OpenAIClient client, IOptions<OpenAiOptions> options, ILogger<EvaluarPuntajeResenia> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    private sealed class PuntajeIaResultado
    {
        public int Puntaje { get; set; }
        public string Motivo { get; set; } = "ninguno";
    }

    public async Task<int> CalcularPuntajeIa(string texto, int estrellas, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return Clamp(estrellas * 20, 0, 100);

        try
        {
            var chatClient = _client.GetChatClient(_options.Model);

            var prompt =
                "Vas a evaluar una reseña de un servicio para decidir si debería mostrarse destacada en la página principal.\n\n" +
                "Respondé EXCLUSIVAMENTE en JSON (sin texto adicional, sin markdown), con esta estructura exacta:\n" +
                "{\n" +
                "  \"puntaje\": 0,\n" +
                "  \"motivo\": \"breve\"\n" +
                "}\n\n" +
                "Reglas:\n" +
                "- puntaje debe ser un entero entre 0 y 100.\n" +
                "- Las estrellas (1 a 5) representan el nivel de satisfacción del usuario y son una señal fuerte.\n" +
                "- Evaluá la coherencia entre las estrellas y el texto.\n" +
                "- Si el texto es genérico pero positivo (ej: \"Excelente\", \"Muy buen servicio\") y coincide con 4 o 5 estrellas, NO penalizarlo fuertemente.\n" +
                "- Penalizá las reseñas genéricas SOLO cuando el texto no justifica las estrellas altas.\n" +
                "- Subí el puntaje cuando el texto es claro, concreto y aporta detalles reales.\n" +
                "- Bajá el puntaje si el texto es negativo, contradictorio, confuso o poco creíble respecto a las estrellas.\n" +
                "- No premiar automáticamente por tener 5 estrellas si el texto no es coherente.\n\n" +
                $"Estrellas: {estrellas}\n" +
                $"Texto: {texto}";


            var messages = new List<ChatMessage>
            {
                new UserChatMessage(prompt)
            };

            ChatCompletion completion = await chatClient.CompleteChatAsync(messages);

            var raw = completion?.Content?.FirstOrDefault()?.Text ?? string.Empty;
            if (string.IsNullOrWhiteSpace(raw))
                return Clamp(estrellas * 20, 0, 100);

            PuntajeIaResultado? parsed = null;
            try
            {
                parsed = JsonSerializer.Deserialize<PuntajeIaResultado>(raw, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                parsed = null;
            }

            if (parsed == null)
                return Clamp(estrellas * 20, 0, 100);

            return Clamp(parsed.Puntaje, 0, 100);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo calcular puntaje IA para reseña. Se usa fallback por estrellas.");
            return Clamp(estrellas * 20, 0, 100);
        }
    }

    private static int Clamp(int value, int min, int max)
        => value < min ? min : (value > max ? max : value);
}