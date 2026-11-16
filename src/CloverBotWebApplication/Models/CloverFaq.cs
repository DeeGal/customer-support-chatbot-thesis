using CloverBotWebApplication.Models;
using Newtonsoft.Json;
using System.IO;

public class CloverFaq
{
    public string? Id { get; set; }
    public string? Category { get; set; }
    public string? Subcategory { get; set; }
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public string[]? Keywords { get; set; }
    public string[]? Tags { get; set; }
    public string[]? Variations { get; set; }
    public string[]? RelatedFaqs { get; set; }
    public string? EscalationType { get; set; }
    public string? BusinessUnit { get; set; }
    public string[]? CustomerType { get; set; }
}

// Wrapper to match your JSON root
public class CloverFaqWrapper
{
    public List<CloverFaq> Faqs { get; set; } = new List<CloverFaq>();
}

public static class CloverFaqLoader
{
    public static List<CloverFaq> LoadCloverFaqs()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "clover_faqs.json");
        if (!File.Exists(path))
            throw new FileNotFoundException("Clover FAQ JSON not found at " + path);

        var json = File.ReadAllText(path);

        // Deserialize the wrapper and return the FAQ list
        var wrapper = JsonConvert.DeserializeObject<CloverFaqWrapper>(json);
        return wrapper?.Faqs ?? new List<CloverFaq>();
    }
}
