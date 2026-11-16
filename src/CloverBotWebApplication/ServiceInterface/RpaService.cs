using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace CloverBotWebApplication.Services
{
    public class RpaService : IRpaService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;

        public RpaService(IHttpClientFactory httpFactory, IConfiguration config)
        {
            _httpFactory = httpFactory;
            _config = config;
        }

        public async Task TriggerRobotAsync(string processName, int? conversationLogId = null)
        {
            // Read UiPath config from appsettings or env vars
            var orchestratorUrl = _config["UiPath:OrchestratorUrl"];
            var apiToken = Environment.GetEnvironmentVariable("UIPATH_API_KEY") ?? _config["UiPath:ApiToken"];

            if (string.IsNullOrWhiteSpace(orchestratorUrl) || string.IsNullOrWhiteSpace(apiToken))
            {
                // Not configured — bail or log
                return;
            }

            var client = _httpFactory.CreateClient();
            var startUrl = $"{orchestratorUrl.TrimEnd('/')}/odata/Jobs/UiPath.Server.Configuration.OData.StartJobs";

            var body = new
            {
                startInfo = new
                {
                    ReleaseName = processName,
                    Strategy = "RobotCount", // or "Specific"
                    NoOfRobots = 1,
                    // pass input arguments if needed
                    InputArguments = conversationLogId.HasValue ? JsonSerializer.Serialize(new { conversationLogId }) : "{}"
                }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, startUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

            var resp = await client.SendAsync(req);
            // You may want to examine response.StatusCode and content for troubleshooting
            resp.EnsureSuccessStatusCode();
        }
    }
}
