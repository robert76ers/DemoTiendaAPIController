using DemoTienda.Infrastructure.Ollama;
using System.Text;
using System.Text.Json;

public interface IDescripcionProductoIAService
{
    Task<string> GenerarDescripcionAsync(string nombreProducto, decimal precio);
}

public class DescripcionProductoIAService : IDescripcionProductoIAService
{
    private readonly HttpClient _httpClient;

    public DescripcionProductoIAService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ollama");
    }

    public async Task<string> GenerarDescripcionAsync(string nombreProducto, decimal precio)
    {
        var prompt = $@"Eres un redactor de marketing.
                        Genera una descripción atractiva en español para el siguiente producto.

                        Nombre: {nombreProducto}
                        Precio: {precio:C}

                        Requisitos:
                        - Máximo 300 caracteres.
                        - Tono profesional pero cercano.
                        - No repitas el nombre más de una vez.
                        - No uses viñetas, solo un párrafo.";

        var request = new OllamaGenerateRequest
        {
            Model = "llama3:latest",
            Prompt = prompt,
            Stream = false
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.PostAsync("/api/generate", content);

        httpResponse.EnsureSuccessStatusCode();

        var responseString = await httpResponse.Content.ReadAsStringAsync();

        var ollamaResponse = JsonSerializer.Deserialize<OllamaGenerateResponse>(responseString);

        var texto = ollamaResponse?.Response?.Trim() ?? string.Empty;

        if (texto.Length > 300)
        {
            texto = texto.Substring(0, 300);
        }

        return texto;
    }
}

