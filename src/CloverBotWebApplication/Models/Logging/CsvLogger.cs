using CloverBotWebApplication.Models.Logging;
using System.Globalization;

public class CsvLogger
{
    private readonly string _filePath;

    public CsvLogger(IWebHostEnvironment env)
    {
        var logDir = Path.Combine(env.WebRootPath, "logs");
        Directory.CreateDirectory(logDir);

        _filePath = Path.Combine(logDir, "execution_logs.csv");

        // Create header if file doesn't exist
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath,
                "TaskType;ExecutionTimeSeconds;ExecutionMode;Phase;UserId;Timestamp;Question;Answer\n");

        }
    }

    public void WriteLog(ExecutionLog log)
    {
        var line =
            $"{log.TaskType};" +
            $"{log.ExecutionTimeSeconds.ToString(CultureInfo.InvariantCulture)};" +
            $"{log.ExecutionMode};" +
            $"{log.Phase};" +
            $"{log.UserId};" +
            $"{log.Timestamp}; " + 
            $"{log.Question}; " +
            $"{log.Answer}; "+
            $"{log.IsCorrect: o}\n";

        File.AppendAllText(_filePath, line);
    }
}
