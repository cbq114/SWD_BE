using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tutor.Applications.Interfaces;

namespace Tutor.Applications.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IConfiguration _configuration;

        public OpenAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiKey = configuration["OpenAI:Key"]
                ?? throw new ArgumentNullException("OpenAI:Key", "OpenAI API key is missing in the configuration.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            // Remove the BaseAddress setting and use the full URL in the request
        }

        public async Task<string> GenerateResponse(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));
            }

            var requestBody = new
            {
                model = "gpt-3.5-turbo",  // Changed to a more commonly available model
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 150  // Added token limit for safety
            };

            var requestContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                // Use the complete URL here
                var response = await _httpClient.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    requestContent
                );

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"OpenAI Response: {responseContent}"); // Debug logging

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonSerializer.Deserialize<OpenAIErrorResponse>(responseContent);

                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests ||
                        errorResponse?.Error?.Code == "insufficient_quota")
                    {
                        throw new OpenAIQuotaExceededException(
                            errorResponse?.Error?.Message ?? "API quota has been exceeded."
                        );
                    }

                    throw new OpenAIException(
                        $"OpenAI API Error: {response.StatusCode}",
                        errorResponse?.Error?.Message ?? responseContent,
                        errorResponse?.Error?.Code
                    );
                }

                var result = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);

                if (result?.Choices == null || !result.Choices.Any())
                {
                    throw new OpenAIException("Empty response", "No content received from OpenAI");
                }

                return result.Choices[0].Message?.Content ?? "No response content";
            }
            catch (HttpRequestException ex)
            {
                throw new OpenAIException("Network Error", ex.Message, null, ex);
            }
            catch (JsonException ex)
            {
                throw new OpenAIException("Response parsing error", ex.Message, null, ex);
            }
        }
    }

    public class OpenAIException : Exception
    {
        public string? ApiErrorMessage { get; }
        public string? ErrorCode { get; }

        public OpenAIException(string message, string? apiErrorMessage = null, string? errorCode = null, Exception? innerException = null)
            : base(message, innerException)
        {
            ApiErrorMessage = apiErrorMessage;
            ErrorCode = errorCode;
        }
    }

    public class OpenAIQuotaExceededException : OpenAIException
    {
        public OpenAIQuotaExceededException(string message)
            : base("OpenAI quota exceeded", message)
        {
        }
    }

    // Response models
    public class OpenAIResponse
    {
        public List<Choice>? Choices { get; set; }
    }

    public class Choice
    {
        public Message? Message { get; set; }
    }

    public class Message
    {
        public string? Content { get; set; }
    }

    public class OpenAIErrorResponse
    {
        public OpenAIError? Error { get; set; }
    }

    public class OpenAIError
    {
        public string? Message { get; set; }
        public string? Type { get; set; }
        public string? Code { get; set; }
    }
}