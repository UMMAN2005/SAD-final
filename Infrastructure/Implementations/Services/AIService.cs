using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Services;

namespace Infrastructure.Implementations.Services;
public class AIService(IConfiguration config) : IAIService {
  public async Task<string?> SendToAIAsync(object prompt) {
    var geminiSettings = config.GetSection("GeminiSettings");

    var apiUrl = geminiSettings["ApiUrl"];
    var apiKey = geminiSettings["ApiKey"];

    var fullUrl = $"{apiUrl}?key={apiKey}";

    var handler = new HttpClientHandler {
      ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };

    using var httpClient = new HttpClient(handler);

    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(prompt), Encoding.UTF8, "application/json");

    var response = await httpClient.PostAsync(fullUrl, content);

    if (!response.IsSuccessStatusCode)
      return await response.Content.ReadAsStringAsync();

    var result = await response.Content.ReadAsStringAsync();
    var aiResponse = System.Text.Json.JsonDocument.Parse(result);

    var text = aiResponse.RootElement
      .GetProperty("candidates")[0]
      .GetProperty("content")
      .GetProperty("parts")[0]
      .GetProperty("text")
      .GetString();

    return text!.StartsWith("EMPTY") ? "" : text;
  }
}
