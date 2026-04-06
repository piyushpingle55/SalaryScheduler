using Hangfire;
using Microsoft.AspNetCore.Mvc;
using SalaryScheduler.BackgroundJobs.Jobs;
using SalaryScheduler.Hangfire.Jobs;

namespace SalaryScheduler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalaryController : ControllerBase
    {
        [HttpPost("trigger")]
        public IActionResult TriggerSalaryJob()
        {
            RecurringJob.Trigger("monthly-salary-job");
            return Ok("Salary job triggered manually.");
        }

        //1) Fire-and-Forget Job: Send notification immediately after salary is processed
        [HttpPost("notify")]
        public IActionResult SendNotification()
        {
            try
            {
                var jobId = BackgroundJob.Enqueue<UtilityJobs>(
                    job => job.SendSalaryNotification(1));

                return Ok(new 
                { 
                    message = "Notification job triggered successfully",
                    jobId = jobId,
                    status = "The email will be sent to pinglepiyush55555@gmail.com",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    message = "Error triggering notification job",
                    error = ex.Message
                });
            }
        }

        //2) Delayed Job :Retry failed payment after 10 minutes
        [HttpPost("retry-payment")]
        public IActionResult RetryPayment()
        {
            BackgroundJob.Schedule<UtilityJobs>(
                job => job.RetryFailedPayment(101),
                TimeSpan.FromMinutes(10));

            return Ok("Retry scheduled after 10 minutes");
        }

        //3) Recurring Job (Already Using) : Monthly salary processing

        //4) Continuation Job : Process salary → then notify employees
        [HttpPost("process-with-notification")]
        public IActionResult ProcessWithNotification()
        {
            var jobId = BackgroundJob.Enqueue<SalaryJob>(
                job => job.Execute());

            BackgroundJob.ContinueJobWith<UtilityJobs>(
                jobId,
                job => job.FinalizeProcess());

            return Ok("Chained job triggered");
        }
    }
}