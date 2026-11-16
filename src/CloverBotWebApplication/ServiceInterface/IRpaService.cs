namespace CloverBotWebApplication.Services
{
    public interface IRpaService
    {
        /// <summary>
        /// Trigger an RPA process in UiPath (or other Orchestrator).
        /// conversationLogId may be null if not needed.
        /// </summary>
        Task TriggerRobotAsync(string processName, int? conversationLogId = null);
    }
}
