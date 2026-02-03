
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using XeniaRentalBackend.Models;

namespace XeniaRentalBackend.Service.Notification
{
    public class OTPService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        public OTPService(ApplicationDbContext context)
        {
            _context = context;      
        }

        public async Task<string> SendNotification(int companyId, int? branchId, string notificationType, string mobileNo, string email, string emailSubject, Dictionary<string, string> parameters)
        {
            var tblNotification = _context.tblNotifications.FirstOrDefault(n => n.CompanyId == companyId && n.NotificationName == notificationType);
            if (tblNotification == null)
                return "0";

            if ((tblNotification.IsSMSEnabled ?? false))
            {
                await SendSMS(companyId, branchId, mobileNo, tblNotification.SMSTemplateId, tblNotification.SMSTemplate, parameters);
            }

            if ((tblNotification.IsEmailEnabled ?? false))
            {
                await SendEmail(companyId, email, emailSubject, tblNotification.EmailTemplate, parameters);
            }

            return "success";
        }


        public async Task<string> SendSMS( int companyId, int? branchId, string mobileNo, string templateId, string template, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(mobileNo))
                return "0";

    
            var smsGatewaySetting = await _context.Set<XRS_CompanySettings>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s =>
                    s.CompanyId == companyId &&
                    s.KeyCode == "SMS_GATEWAY" &&
                    s.Active);

            if (smsGatewaySetting == null || string.IsNullOrWhiteSpace(smsGatewaySetting.Value))
                return "0";

            try
            {
                string url = smsGatewaySetting.Value;

                url = url.Replace("{mobileNo}", mobileNo)
                         .Replace("{templateId}", templateId ?? string.Empty)
                         .Replace("{message}", ReplaceVariables(template, parameters));

                var request = WebRequest.CreateHttp(url);
                request.Method = "GET";

                using var response = await request.GetResponseAsync();
                using var sr = new StreamReader(response.GetResponseStream()!);

                string results = await sr.ReadToEndAsync();
                return results;
            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        public async Task<string> SendEmail( int companyId, string email,  string subject, string template,  Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "0";

            var settings = await _context.Set<XRS_CompanySettings>()
                .AsNoTracking()
                .Where(s => s.CompanyId == companyId && s.Active)
                .ToDictionaryAsync(s => s.KeyCode, s => s.Value);

            if (!settings.TryGetValue("EMAIL_HOST", out var host) ||
                !settings.TryGetValue("EMAIL_PORT", out var portStr) ||
                !settings.TryGetValue("EMAIL_SENDER", out var sender) ||
                !settings.TryGetValue("EMAIL_PASSWORD", out var password))
            {
                return "0";
            }

            bool enableSsl = false;
            if (settings.TryGetValue("EMAIL_SSL", out var sslValue))
                bool.TryParse(sslValue, out enableSsl);

            if (!int.TryParse(portStr, out int port))
                return "0";

            try
            {
                using var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(sender, password),
                    EnableSsl = enableSsl
                };

                var message = new MailMessage
                {
                    From = new MailAddress(sender),
                    Subject = subject,
                    Body = ReplaceVariables(template, parameters),
                    IsBodyHtml = true
                };

                message.To.Add(email);

                await client.SendMailAsync(message);
                return "success";
            }
            catch (Exception ex)
            {
                return "0";
            }
        }


        public string ReplaceVariables(string template, Dictionary<string, string> parameters)
        {
            foreach (var item in parameters)
            {
                template = template.Replace(item.Key, item.Value);
            }
            return template;
        }
    }
}
