using CloverBotWebApplication.Models;
using CloverBotWebApplication.ServiceInterface;
using CloverBotWebApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloverBotWebApplication.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] ChatRequest req)
        {
            var result = await _chatService.GetBotResponseAsync(req.Message);
            return Json(new ChatResponse { Response = result });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
