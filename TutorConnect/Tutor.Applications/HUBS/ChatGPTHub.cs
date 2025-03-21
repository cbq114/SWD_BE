using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Tutor.Applications.Interfaces;

namespace Tutor.Applications.HUBS
{
    [Authorize]
    public class ChatGPTHub : Hub
    {
        private readonly IOpenAIService _openAIService;

        public ChatGPTHub(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public async Task<string> GenerateChatGPTResponse(string prompt)
        {
            try
            {
                var response = await _openAIService.GenerateResponse(prompt);
                return response;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }


}
