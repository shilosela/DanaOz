using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace DanaOz.BLL.Services
{
    public class AIService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private const string GeminiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

        public AIService(IConfiguration configuration)
        {
            _apiKey = configuration["Gemini:ApiKey"]!;
            _httpClient = new HttpClient();
        }

        public async Task<string> GenerateResponseAsync(string userMessage, string teacherContext = "")
        {
            try
            {
                var systemPrompt = BuildSystemPrompt(teacherContext);
                var fullPrompt = systemPrompt + "\n\nהמורה כתב: " + userMessage;

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = fullPrompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 1024
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    GeminiUrl + "?key=" + _apiKey, content);

                var responseString = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseString);

                var text = responseJson
                    .RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? GetErrorMessage();
            }
            catch (Exception)
            {
                return GetErrorMessage();
            }
        }

        public async Task<string> GenerateMaterialAsync(string request, string teacherContext = "")
        {
            var prompt = "אתה דנה עוז, עוזרת הוראה חכמה למורים.\n" +
                        teacherContext + "\n" +
                        "המורה ביקש: " + request + "\n" +
                        "אנא הפק את החומר המבוקש בעברית תקנית, מקצועי ומפורט.";

            return await GenerateResponseAsync(prompt);
        }

        private string BuildSystemPrompt(string teacherContext)
        {
            return "אתה דנה עוז - עוזרת אישית חכמה למורים.\n" +
                   "האישיות שלך: חמה, אמפתית, מקצועית, תמציתית ומעודדת.\n" +
                   "השפה: עברית תקנית בלבד.\n" +
                   "התפקיד שלך: לעזור למורים בהכנת חומרי לימוד, מערכי שיעור, מבחנים ותזכורות.\n" +
                   teacherContext + "\n" +
                   "חשוב: תשובות קצרות וממוקדות. אל תכתוב יותר ממה שנדרש.";
        }

        private string GetErrorMessage()
        {
            return "אוי, סליחה! חוויתי עומס רגעי. אנא נסה שוב בעוד כמה שניות";
        }
    }
}