
using System.Net.Mail;
using System.Net;
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


        public async Task<string> SendSMS(int companyId, int? branchId, string mobileNo, string templateId, string template, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(mobileNo))
                return "0";

            var tblEmailSmsSettings = _context.tblEmailSmsSettings.FirstOrDefault(e => e.companyID == companyId);
            if (tblEmailSmsSettings?.smsGateWay == null || string.IsNullOrWhiteSpace(tblEmailSmsSettings.smsGateWay))
                return "0";

            try
            {
                string url = tblEmailSmsSettings.smsGateWay;
                url = url.Replace("{mobileNo}", mobileNo)
                         .Replace("{templateId}", templateId ?? "")
                         .Replace("{message}", ReplaceVariables(template, parameters));

                var request = WebRequest.CreateHttp(url);
                using var response = await request.GetResponseAsync();
                using var sr = new StreamReader(response.GetResponseStream());
                string results = await sr.ReadToEndAsync();

                return results;
            }
            catch
            {
                return "0";
            }
            return "success";
        }

        public async Task<string> SendEmail(int companyId, string email, string subject, string template, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "0";

            var settings = _context.tblEmailSmsSettings.FirstOrDefault(e => e.companyID == companyId);
            if (settings == null || string.IsNullOrWhiteSpace(settings.emailSender) || string.IsNullOrWhiteSpace(settings.password) ||
                string.IsNullOrWhiteSpace(settings.host) || string.IsNullOrWhiteSpace(settings.port))
                return "0";

            try
            {
                using var client = new SmtpClient(settings.host, int.Parse(settings.port))
                {
                    Credentials = new NetworkCredential(settings.emailSender, settings.password),
                    EnableSsl = settings.enableSsl ?? false
                };

                var message = new MailMessage
                {
                    From = new MailAddress(settings.emailSender),
                    Subject = subject,
                    Body = ReplaceVariables(template, parameters),
                    IsBodyHtml = true
                };
                message.To.Add(email);

                await client.SendMailAsync(message);
                return "success";
            }
            catch
            {
                return "0";
            }
            return "success";
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
