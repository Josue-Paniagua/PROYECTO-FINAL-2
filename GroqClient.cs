using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Proyecto_Final_1
{
    public class GroqClient
    {
        private readonly HttpClient _http = new();

        public GroqClient(string apiKey)
        {
            _http.BaseAddress = new System.Uri("https://api.groq.com/openai/v1/");
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<string> ChatAsync(string prompt)
        {
               
            var body = new
            {
                model = "llama3-8b-8192",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("chat/completions", content);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var json = await JsonDocument.ParseAsync(stream);
            return json.RootElement.GetProperty("choices")[0]
                                   .GetProperty("message")
                                   .GetProperty("content")
                                   .GetString();
        }

    }
}
