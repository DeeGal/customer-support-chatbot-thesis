using CloverBotWebApplication.Models;
using CloverBotWebApplication.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace CloverBotWebApplication.Controllers
{
    [ApiController]
    [Route("api/chatbot")]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatbotController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("ask")]
        public async Task<ChatResponse> Ask([FromBody] ChatRequest request)
        {
            var botReply = await _chatService.GetBotResponseAsync(request.Message, request.UserId);
            return new ChatResponse { Response = botReply };
        }
    }
}
