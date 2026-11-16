namespace CloverBotWebApplication.Models.Logging
{
    public class ExecutionLog
    {
        public string? TaskType { get; set; }
        public double ExecutionTimeSeconds { get; set; }
        public string? ExecutionMode { get; set; } // Human | Chatbot | RPA
        public string? Phase { get; set; } // Baseline | Automated
        public string? UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
        public bool IsCorrect { get; set; }  

    }

}
