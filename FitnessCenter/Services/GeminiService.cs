using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FitnessCenter.Models;
using Microsoft.Extensions.Options;

namespace FitnessCenter.Services
{
    public class GeminiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GeminiOptions _options;

        public GeminiService(IHttpClientFactory httpClientFactory, IOptions<GeminiOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public async Task<string> GetWorkoutRecommendationAsync(string goal, string level, string preferences)
        {
            var client = _httpClientFactory.CreateClient();

            var url = $"{_options.Endpoint}?key={_options.ApiKey}";

            var prompt =
                $"Sen deneyimli bir kişisel antrenörsün. " +
                $"Kullanıcının hedefi: {goal}. Seviye: {level}. " +
                $"Tercihleri / notları: {preferences}. " +
                $"Kişiye özel, 1 haftalık detaylı spor programı yaz. " +
                $"Gün gün listele, set/tekrar sayısı ver ve motive edici Türkçe kullan.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text ?? "Şu anda öneri üretilemedi.";
        }
    }
}
