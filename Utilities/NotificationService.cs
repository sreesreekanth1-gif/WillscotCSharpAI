using Framework.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Framework.Utilities
{
    public static class NotificationService
    {
        public static void SendEmail(int total, int passed, int failed, string duration, string reportUrl = "")
        {
            if (!ConfigReader.IsEmailEnabled())
            {
                Console.WriteLine("Email notifications are disabled (dry run).");
                return;
            }

            var settings = ConfigReader.EmailSettings();

            using var mail = new MailMessage();
            mail.From = new MailAddress(settings.SenderEmail);
            mail.To.Add(settings.RecipientEmail);
            mail.Subject = $"Test Run Summary - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
            mail.IsBodyHtml = true;

            // Create HTML body with test summary
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 10px; text-align: center; }}
        .summary {{ margin: 20px 0; }}
        .summary table {{ border-collapse: collapse; width: 100%; }}
        .summary th, .summary td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
        .summary th {{ background-color: #f2f2f2; }}
        .passed {{ color: green; font-weight: bold; }}
        .failed {{ color: red; font-weight: bold; }}
        .total {{ color: blue; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='header'>
        <h2>Test Execution Summary</h2>
    </div>
    <div class='summary'>
        <table>
            <tr><th>Metric</th><th>Value</th></tr>
            <tr><td>Total Tests</td><td class='total'>{total}</td></tr>
            <tr><td>Passed</td><td class='passed'>{passed}</td></tr>
            <tr><td>Failed</td><td class='failed'>{failed}</td></tr>
            <tr><td>Duration</td><td>{duration}</td></tr>
            <tr><td>Report URL</td><td>{reportUrl}</td></tr>
        </table>
    </div>
    <p>Please find the detailed Allure report attached.</p>
</body>
</html>";

            mail.Body = htmlBody;

            // Attach index.html if it exists
            var indexHtmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Reports", "index.html");
            indexHtmlPath = Path.GetFullPath(indexHtmlPath);
            
            if (File.Exists(indexHtmlPath))
            {
                try
                {
                    var attachment = new Attachment(indexHtmlPath, MediaTypeNames.Text.Html);
                    if (attachment.ContentDisposition != null)
                    {
                        attachment.ContentDisposition.FileName = "Allure-Report.html";
                    }
                    mail.Attachments.Add(attachment);
                    Console.WriteLine($"Attached report: {indexHtmlPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Failed to attach report: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Warning: Report file not found at {indexHtmlPath}");
            }

            using var smtp = new SmtpClient(settings.SmtpServer, settings.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(settings.SenderEmail, settings.SenderPassword)
            };
            smtp.Send(mail);
            Console.WriteLine("Email sent successfully!");
        }

        public static async Task SendSlackMessage(int total, int passed, int failed, string duration, string reportUrl = "")
        {
            var settings = ConfigReader.SlackSettings();
            if (!settings.EnableSlack)
            {
                Console.WriteLine("Slack notifications are disabled.");
                return;
            }

            var payload = new { text = $"*Test Run Completed*\nTotal: {total}\nPassed: {passed}\nFailed: {failed}\nDuration: {duration}\nReport: {reportUrl}" };
            await PostJsonAsync(settings.Webhook, payload);
        }

        public static async Task SendTeamsMessage(int total, int passed, int failed, string duration, string reportUrl = "")
        {
            var settings = ConfigReader.TeamsSettings();
            if (!settings.EnableTeams)
            {
                Console.WriteLine("Teams notifications are disabled.");
                return;
            }

            var payload = new
            {
                title = "Test Run Completed",
                text = $"Total: {total}\nPassed: {passed}\nFailed: {failed}\nDuration: {duration}\nReport: {reportUrl}"
            };
            await PostJsonAsync(settings.Webhook, payload);
        }

        private static async Task PostJsonAsync(string url, object payload)
        {
            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
