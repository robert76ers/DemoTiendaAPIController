using System.Text.Json.Serialization;

namespace DemoTienda.Infrastructure.Ollama
{
    public class OllamaGenerateRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "llama3:latest";

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; } = string.Empty;

        [JsonPropertyName("stream")]
        public bool Stream { get; set; } = false;
    }

    public class OllamaGenerateResponse
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("response")]
        public string Response { get; set; } = string.Empty;

        [JsonPropertyName("done")]
        public bool Done { get; set; }
    }

}
