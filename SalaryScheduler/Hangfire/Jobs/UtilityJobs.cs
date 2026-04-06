using Microsoft.Extensions.Logging;
using SalaryScheduler.Application.Services;

namespace SalaryScheduler.Hangfire.Jobs
{
    public class UtilityJobs
    {
        private readonly ILogger<UtilityJobs> _logger;
        private readonly IEmailService _emailService;

        public UtilityJobs(ILogger<UtilityJobs> logger, IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        //Fire-and-Forget
        public async Task SendSalaryNotification(int employeeId)
        {
            try
            {
                _logger.LogInformation($"Starting notification job for Employee {employeeId}");
                
                // This is a sample email - in production, you would fetch employee details from database
                string employeeEmail = "pinglepiyush55555@gmail.com";
                string employeeName = $"Employee {employeeId}";
                decimal salary = 5000m; // Sample salary amount
                
                await _emailService.SendSalaryNotificationAsync(
                    employeeEmail, 
                    employeeName, 
                    salary, 
                    DateTime.Now);

                _logger.LogInformation($"Notification email sent successfully to Employee {employeeId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification to Employee {employeeId}: {ex.Message}");
                throw;
            }
        }

        //Delayed
        public void RetryFailedPayment(int salaryId)
        {
            _logger.LogInformation($"Retrying payment for SalaryId {salaryId}");
        }

        //Recurring
        public void GenerateSalaryReport()
        {
            _logger.LogInformation("Generating monthly salary report...");
        }

        //Continuation
        public void FinalizeProcess()
        {
            _logger.LogInformation("Final step completed after salary processing.");
        }
    }
}