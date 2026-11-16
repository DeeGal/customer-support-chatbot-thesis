using CloverBotWebApplication.Models;
using CloverBotWebApplication.Models.Logging;
using CloverBotWebApplication.ServiceInterface;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class ChatService : IChatService
{
    private readonly List<FaqItem> _faqs;
    private readonly CsvLogger _logger;

    public ChatService(IWebHostEnvironment env, CsvLogger logger)
    {
        var filePath = Path.Combine(env.WebRootPath, "data", "clover_faqs.json");
        var json = File.ReadAllText(filePath);

        var dataset = JsonConvert.DeserializeObject<FaqDataset>(json);
        _faqs = dataset?.Faqs ?? new List<FaqItem>();
        _logger = logger;
    }

    public Task<string> GetBotResponseAsync(string message, string? userId = "Anonymous")
    {
        userId ??= "Anonymous";
        var sw = Stopwatch.StartNew();

        if (string.IsNullOrWhiteSpace(message))
        {
            Log(sw, "EmptyMessage", userId, message, "");
            return Task.FromResult("Please type a message.");
        }

        var lowerMsg = message.ToLower();
        var messageWords = Regex.Split(lowerMsg, @"\W+")
            .Where(w => !string.IsNullOrEmpty(w))
            .ToList();

        int GetScore(FaqItem faq)
        {
            int score = 0;

            if (faq.Question?.ToLower() == lowerMsg) score += 3;

            if (faq.Variations != null &&
                faq.Variations.Any(v => v.ToLower() == lowerMsg))
                score += 2;

            if (faq.Keywords != null)
                foreach (var kw in faq.Keywords)
                    if (messageWords.Contains(kw.ToLower()))
                        score++;

            var faqWords = Regex.Split(faq.Question?.ToLower() ?? "",
                    @"\W+").Where(w => !string.IsNullOrEmpty(w));

            score += faqWords.Count(w => messageWords.Contains(w));

            if (faq.Variations != null)
            {
                foreach (var variation in faq.Variations)
                {
                    var varWords = Regex.Split(variation.ToLower(), @"\W+")
                        .Where(w => !string.IsNullOrEmpty(w));

                    score += varWords.Count(w => messageWords.Contains(w));
                }
            }

            return score;
        }

        var bestMatch = _faqs
            .Select(f => new { Faq = f, Score = GetScore(f) })
            .OrderByDescending(f => f.Score)
            .FirstOrDefault();

        int threshold = 3;

        string reply;

        if (bestMatch != null && bestMatch.Score >= threshold)
        {
            reply = bestMatch.Faq.Answer;
            Log(sw, "FAQ_Matched", userId, message, reply, bestMatch.Faq.Answer); 
        }
        else if (bestMatch != null)
        {
            reply = $"Did you mean: \"{bestMatch.Faq.Question}\"?";
            Log(sw, "FAQ_Suggested", userId, message, reply);
        }
        else
        {
            reply = "Sorry, I couldn't find an answer. Can you rephrase your question?";
            Log(sw, "NoMatch", userId, message, reply);
        }

        return Task.FromResult(reply);
    }

    private void Log(
    Stopwatch sw,
    string taskType,
    string userId,
    string question,
    string answer,
    string? correctAnswer = null)
    {
        try
        {
            sw.Stop();

            bool isCorrect = false;
            if (!string.IsNullOrEmpty(correctAnswer))
            {
                isCorrect = string.Equals(answer.Trim(), correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
            }

            _logger.WriteLog(new CloverBotWebApplication.Models.Logging.ExecutionLog
            {
                TaskType = taskType,
                ExecutionTimeSeconds = sw.Elapsed.TotalSeconds,
                ExecutionMode = "Chatbot",
                Phase = "Automated",
                UserId = userId,
                Timestamp = DateTime.UtcNow,
                Question = question,
                Answer = answer,
                IsCorrect = isCorrect
            });
        }
        catch { }
    }
}

// Models
public class FaqDataset
{
    [JsonProperty("faqs")]
    public List<FaqItem>? Faqs { get; set; }
}

public class FaqItem
{
    public string? Id { get; set; }
    public string? Category { get; set; }
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public List<string>? Keywords { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? Variations { get; set; }
}
