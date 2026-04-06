using Microsoft.Extensions.Logging;

namespace SalaryScheduler.Hangfire.Jobs
{
    public class UtilityJobs
    {
        private readonly ILogger<UtilityJobs> _logger;

        public UtilityJobs(ILogger<UtilityJobs> logger)
        {
            _logger = logger;
        }
        //Fire-and-Forget
        public void SendSalaryNotification(int employeeId)
        {
            _logger.LogInformation($"Notification sent to Employee {employeeId}");
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