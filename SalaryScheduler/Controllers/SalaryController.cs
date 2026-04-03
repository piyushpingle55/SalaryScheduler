using Hangfire;
using Microsoft.AspNetCore.Mvc;

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
    }
}