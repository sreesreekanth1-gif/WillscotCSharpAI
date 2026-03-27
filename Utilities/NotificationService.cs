using Framework.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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
            mail.Body = $"Total: {total}\nPassed: {passed}\nFailed: {failed}\nDuration: {duration}\nReport: {reportUrl}";

            using var smtp = new SmtpClient(settings.SmtpServer, settings.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(settings.SenderEmail, settings.SenderPassword)
            };
            smtp.Send(mail);
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
