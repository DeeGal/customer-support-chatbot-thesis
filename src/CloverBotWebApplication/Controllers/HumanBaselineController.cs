using CloverBotWebApplication.Models.Logging;
using Microsoft.AspNetCore.Mvc;
using CloverBotWebApplication.Models;
using System.Globalization;

public class HumanBaselineController : Controller
{
    private readonly IWebHostEnvironment _env;

    public HumanBaselineController(IWebHostEnvironment env)
    {
        _env = env;
    }
    //public IActionResult Index(int index = 0)
    //{
    //    var logsPath = Path.Combine(_env.WebRootPath, "logs", "execution_logs.csv");
    //    if (!System.IO.File.Exists(logsPath))
    //        return Content("Automated log file not found.");

    //    var lines = System.IO.File.ReadAllLines(logsPath)
    //                    .Skip(1)
    //                    .ToList();

    //    if (index >= lines.Count)
    //        return View("Completed");

    //    var separator = lines[index].Contains(";") ? ';' : ',';
    //    var parts = lines[index].Split(separator);

    //    var model = new HumanBaselineViewModel
    //    {
    //        Index = index,
    //        TaskType = parts.Length > 0 ? parts[0].Trim() : "",
    //        Question = parts.Length > 6 ? parts[6].Trim() : "No question available",
    //        ChatbotAnswer = parts.Length > 7 ? string.Join(separator, parts.Skip(7)).Trim() : "",
    //        StartTime = DateTime.UtcNow.ToString("o"), // store start time
    //        Answer = "" // always empty for human input
    //    };

    //    return View(model);
    //}

    public IActionResult Index(int index = 0)
    {

        var logsPath = Path.Combine(_env.WebRootPath, "logs", "execution_logs.csv");

        if (!System.IO.File.Exists(logsPath))
            return Content("Automated log file not found.");

        var lines = System.IO.File.ReadAllLines(logsPath)
                        .Skip(1) // skip header
                        .ToList();

        if (index >= lines.Count)
            return View("Completed");


        var separator = lines[index].Contains(";") ? ';' : ',';
        var parts = lines[index].Split(separator);

        var model = new HumanBaselineViewModel
        {
            Index = index,
            TaskType = parts.Length > 0 ? parts[0].Trim() : "",
            Question = parts.Length > 6 ? parts[6].Trim() : "No question available",
            Answer = parts.Length > 7 ? string.Join(separator, parts.Skip(7)).Trim() : ""
        };


        //Console.WriteLine("Line raw: " + lines[index]);
        //Console.WriteLine("Separator: " + separator);
        //for (int i = 0; i < parts.Length; i++)
        //{
        //    Console.WriteLine($"parts[{i}] = '{parts[i]}'");
        //}


        //var model = new HumanBaselineViewModel
        //{
        //    Index = index,
        //    TaskType = parts.Length > 0 ? parts[0].Trim() : "",
        //    Question = question,
        //    Answer = chatbotAnswer // prefill with chatbot answer
        //};

        return View(model);
    }

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult Save(HumanBaselineViewModel model)
    //{
    //    var baselinePath = Path.Combine(_env.WebRootPath, "logs", "execution_logs_baseline.csv");

    //    if (!System.IO.File.Exists(baselinePath))
    //    {
    //        System.IO.File.WriteAllText(baselinePath,
    //            "TaskType;ExecutionTimeSeconds;ExecutionMode;Phase;UserId;Timestamp;Question;Answer\n");
    //    }

    //    // Escape semicolons in Question/Answer
    //    string safeQuestion = model.Question?.Replace(";", ",") ?? "";
    //    string safeAnswer = model.Answer?.Replace(";", ",") ?? "";

    //    var line =
    //        $"{model.TaskType};0;Human;Baseline;Human;" +
    //        $"{DateTime.UtcNow:o};{safeQuestion};{safeAnswer}\n";

    //    System.IO.File.AppendAllText(baselinePath, line);

    //    return RedirectToAction("Index", new { index = model.Index + 1 });
    //}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Save(HumanBaselineViewModel model)
    {
        var baselinePath = Path.Combine(_env.WebRootPath, "logs", "execution_logs_baseline.csv");

        if (!System.IO.File.Exists(baselinePath))
        {
            System.IO.File.WriteAllText(baselinePath,
                "ExecutionTimeSeconds;ExecutionMode;Phase;UserId;Timestamp;Question;Answer;IsCorrect\n");
        }

        // Parse StartTime safely using ISO 8601 format
        DateTime startTime;
        if (!DateTime.TryParseExact(model.StartTime, "o", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out startTime))
        {
            startTime = DateTime.UtcNow;
        }

        // Calculate response time in seconds
        double executionTimeSeconds = (DateTime.UtcNow - startTime).TotalSeconds;

        // Escape semicolons in Question/Answer
        string safeQuestion = model.Question?.Replace(";", ",") ?? "";
        string safeAnswer = model.Answer?.Replace(";", ",") ?? "";

        // Compare human answer to chatbot answer (case-insensitive)
        bool isCorrect = string.Equals(safeAnswer.Trim(), model.ChatbotAnswer?.Trim(), StringComparison.OrdinalIgnoreCase);

        // Build CSV line using invariant culture
        var line = string.Format(CultureInfo.InvariantCulture,
            "{0:F6};Human;Baseline;Human;{1:o};{2};{3};{4}\n",
            executionTimeSeconds,
            DateTime.UtcNow,
            safeQuestion,
            safeAnswer,
            isCorrect);

        System.IO.File.AppendAllText(baselinePath, line);

        Console.WriteLine("🔥 Human response saved 🔥");
        Console.WriteLine($"Question: {model.Question}");
        Console.WriteLine($"Answer: {model.Answer}");
        Console.WriteLine($"ExecutionTimeSeconds: {executionTimeSeconds:F6}");
        Console.WriteLine($"IsCorrect: {isCorrect}");

        return RedirectToAction("Index", new { index = model.Index + 1 });
    }


}
