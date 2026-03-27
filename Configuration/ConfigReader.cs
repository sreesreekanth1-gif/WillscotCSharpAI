using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Framework.Configuration
{
    public static class ConfigReader
    {
        private static readonly IConfiguration _configuration;

        static ConfigReader()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Configuration/appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public static string Get(string key)
            => _configuration[key] ?? throw new InvalidOperationException($"Configuration key '{key}' not found.");

        public static T GetValue<T>(string key)
            => _configuration.GetValue<T>(key);

        public static string Environment => Get("Environment");
        public static string ReportTitle => Get("ReportTitle");
        public static string BaseUrl => Get("BaseUrl");
        public static SlackSettings SlackSettings()
        {
            var section = _configuration.GetSection("SlackSettings");
            return section.Get<SlackSettings>() ?? throw new InvalidOperationException("SlackSettings not configured.");
        }

        public static TeamsSettings TeamsSettings()
        {
            var section = _configuration.GetSection("TeamsSettings");
            return section.Get<TeamsSettings>() ?? throw new InvalidOperationException("TeamsSettings not configured.");
        }

        public static EmailSettings EmailSettings()
        {
            var section = _configuration.GetSection("EmailSettings");
            return section.Get<EmailSettings>() ?? throw new InvalidOperationException("EmailSettings not configured.");
        }

        public static bool IsEmailEnabled()
            => _configuration.GetValue<bool>("EmailSettings:EnableEmail", false);

        public static ApiEndpoints ApiEndpoints()
        {
            var section = _configuration.GetSection("ApiEndpoints");
            return section.Get<ApiEndpoints>() ?? throw new InvalidOperationException("ApiEndpoints not configured.");
        }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
    }

    public class ApiEndpoints
    {
        public string LandingPage { get; set; } = string.Empty;
    }

    public class SlackSettings
    {
        public bool EnableSlack { get; set; }
        public string Webhook { get; set; } = string.Empty;
    }

    public class TeamsSettings
    {
        public bool EnableTeams { get; set; }
        public string Webhook { get; set; } = string.Empty;
    }
}