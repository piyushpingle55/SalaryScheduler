using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalaryScheduler.Repository;

namespace SalaryScheduler.Application.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task SendSalaryNotificationAsync(string employeeEmail, string employeeName, decimal salary, DateTime processDate);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailSettingsRepository _emailSettingsRepository;

        public EmailService(ILogger<EmailService> logger,IEmailSettingsRepository emailSettingsRepository)
        {
            _logger = logger;
            _emailSettingsRepository = emailSettingsRepository;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            // call to db Email Settings Table.
            var senderEmailDetail = await _emailSettingsRepository.GetActiveSettingsAsync();
            var SmtpPort  = senderEmailDetail.SmtpPort ?? 587;
            var EnableSsl = senderEmailDetail.EnableSsl ?? true;
            try
            {
                if (string.IsNullOrEmpty(senderEmailDetail.SenderEmail) || string.IsNullOrEmpty(senderEmailDetail.SenderPassword))
                {
                    _logger.LogWarning("Email service not configured. Skipping email to {ToEmail}", toEmail);
                    return;
                }

                using (SmtpClient smtpClient = new SmtpClient(senderEmailDetail.SmtpServer, SmtpPort))
                {
                    smtpClient.EnableSsl = EnableSsl;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(senderEmailDetail.SenderEmail, senderEmailDetail.SenderPassword);

                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(senderEmailDetail.SenderEmail, "Salary Scheduler");
                        mailMessage.To.Add(toEmail);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = isHtml;

                        await smtpClient.SendMailAsync(mailMessage);
                        _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
                    }
                }
            }
            catch (SmtpException ex)
            {
                _logger.LogError($"SMTP error sending email to {toEmail}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {toEmail}: {ex.Message}");
                throw;
            }
        }

        public async Task SendSalaryNotificationAsync(string employeeEmail, string employeeName, decimal salary, DateTime processDate)
        {
            try
            {
                string subject = "Salary Notification - Payment Processed";

                string htmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px; }}
                            .content {{ background-color: #f9f9f9; padding: 20px; margin: 20px 0; border-left: 4px solid #4CAF50; }}
                            .footer {{ text-align: center; color: #999; font-size: 12px; margin-top: 30px; }}
                            .amount {{ font-size: 24px; font-weight: bold; color: #4CAF50; }}
                        }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>Salary Payment Notification</h1>
                            </div>
                            <div class='content'>
                                <p>Dear <strong>{employeeName}</strong>,</p>
                                <p>Your salary has been successfully processed and will be transferred to your account.</p>
                                <p>
                                    <strong>Salary Amount:</strong> <span class='amount'>${salary:F2}</span>
                                </p>
                                <p>
                                    <strong>Processing Date:</strong> {processDate:MMMM dd, yyyy}
                                </p>
                                <p style='margin-top: 20px; color: #666;'>
                                    If you have any questions regarding your salary or payment, please contact the HR department.
                                </p>
                            </div>
                            <div class='footer'>
                                <p>This is an automated message from Salary Scheduler. Please do not reply to this email.</p>
                                <p>&copy; 2026 Employee Salary Scheduler. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                await SendEmailAsync(employeeEmail, subject, htmlBody, isHtml: true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendSalaryNotificationAsync: {ex.Message}");
                throw;
            }
        }
    }
}
