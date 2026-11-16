namespace CloverBotWebApplication.Models
{
    public class ChatRequest
    {
        public string? Message { get; set; }
        public string? UserId { get; internal set; }
    }
}
