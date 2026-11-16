namespace CloverBotWebApplication.ServiceInterface
{

    public interface IChatService
    {
        Task<string> GetBotResponseAsync(string userMessage, string userId = null);
    }
}
