//using CsvHelper;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;

//namespace CloverBotWebApplication.ServiceInterface
//{
//    // Local ExecutionLog for baseline CSV
//    public class ExecutionLog
//    {
//        public string TaskType { get; set; } = "";
//        public double ExecutionTimeSeconds { get; set; }
//        public string ExecutionMode { get; set; } = "";
//        public string Phase { get; set; } = "";
//        public string UserId { get; set; } = "";
//        public DateTime Timestamp { get; set; }
//        public string Question { get; set; } = "";
//        public string Answer { get; set; } = "";
//    }

//    public static class HumanBaselineGenerator
//    {
//        public static void GenerateBaseline(string automatedCsvPath, string outputCsvPath)
//        {
//            // Read automated logs
//            using var reader = new StreamReader(automatedCsvPath);
//            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
//            var automatedLogs = csvReader.GetRecords<ExecutionLog>().ToList();

//            var humanLogs = new List<ExecutionLog>();

//            foreach (var log in automatedLogs)
//            {
//                Console.WriteLine($"\nQuestion: {log.Question}");
//                Console.Write("Your Answer: ");
//                var humanAnswer = Console.ReadLine() ?? "";

//                humanLogs.Add(new ExecutionLog
//                {
//                    TaskType = log.TaskType,
//                    ExecutionTimeSeconds = log.ExecutionTimeSeconds,
//                    ExecutionMode = "Human",
//                    Phase = "Baseline",
//                    UserId = log.UserId,
//                    Timestamp = DateTime.UtcNow,
//                    Question = log.Question,
//                    Answer = humanAnswer
//                });
//            }

//            // Write human baseline CSV
//            using var writer = new StreamWriter(outputCsvPath);
//            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
//            csvWriter.WriteRecords(humanLogs);

//            Console.WriteLine($"\n✅ Human baseline CSV saved to: {outputCsvPath}");
//        }
//    }

//    // Console entry point
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            string logsFolder = Path.Combine(AppContext.BaseDirectory, "logs");
//            Directory.CreateDirectory(logsFolder); // ensures folder exists
                 
//            string automatedCsvPath = Path.Combine(logsFolder, "execution_logs.csv");
//            string outputCsvPath = Path.Combine(logsFolder, "execution_logs_baseline.csv");

//            HumanBaselineGenerator.GenerateBaseline(
//                automatedCsvPath: automatedCsvPath,
//                outputCsvPath: outputCsvPath
//             );
//        }
//    }
//}
// https://localhost:7258/HumanBaseline
